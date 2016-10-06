=head1 NAME

CallEBayAPI - Provides simple perl module to invoke ebay API calls

=head1 SYNOPSIS

  use CallEBayAPI;
  # Create the eBay interface
  my $eBayInterface = CallEBayAPI->new( DEV_KEY, APP_KEY, CERT_KEY );
  # Create an API request object that will handle your request
  my $eBayRequest = $eBayInterface->newRequest(
									  {
									   Verb => 'GetLogoURL',
									   Size => 'Medium',
									   RequestUserId => USER
									   RequestPassword => PASSWORD
                                      } );
  # Set the fields in the request
  # Issue the request
  $eBayResponse=$eBayInterface->request( $eBayRequest );
  if ( $eBayResponse->{Errors} )
  {
    # handle any errors
  }
  # The results are just a hash!

=head1 DESCRIPTION

This package provides a simple interface to invoke arbitrary
eBay API calls by wrapping the necessary HTTPS transactions and
minimizing the amount of XML your application must manage.

=head1 COPYRIGHT

Copyright (C) 2003 Ray Rischpater. All rights reserved.

This program is free software; you can redistribute and/or modify
it under the same terms as Perl itself as long as the copyright
attribution remains.

This file is provided without any warranty, express or
implied, including but not limited to fitness of purpose.

This file is part of the book "Developing Applications with
eBay" written by Ray Rischpater and available from
Apress, LP.

=cut;

package CallEBayAPI;

# dependencies
use LWP::UserAgent;
use HTTP::Request;
use HTTP::Headers;
use XML::Smart;
use Data::Dumper;

use strict;

# constants
use constant True => 1;
use constant False => 0;
use constant COMPATIBILITY_LEVEL => 327;
use constant EBAY_API_URL => 'https://api.sandbox.ebay.com/ws/api.dll';
use constant DEBUG => 0;
use constant SITE_ID => '0';
use constant DETAIL_LEVEL => '0';
use constant SiteIDEnum_US => 0;
use constant SiteIDEnum_Canada => 1;
use constant SiteIDEnum_UnitedKingdom => 2;
use constant SiteIDEnum_Australia => 3;
use constant SiteIDEnum_Austria => 4;
use constant SiteIDEnum_BelgiumFrench => 5;
use constant SiteIDEnum_France => 6;
use constant SiteIDEnum_Germany => 7;
use constant SiteIDEnum_Italy => 8;
use constant SiteIDEnum_BelgiumDutch => 9;
use constant SiteIDEnum_Netherlands => 10;
use constant SiteIDEnum_Spain => 11;
use constant SiteIDEnum_Switzerland => 12;
use constant SiteIDEnum_Taiwan => 13;
use constant SiteIDEnum_eBayMotors => 14;

use constant EnvironmentEnum_SANDBOX => 0;
use constant EnvironmentEnum_PRODUCTION => 1;
use constant EnvironmentEnum_QA => 2;

# module variables
my $_xmlRequest = '<?xml version="1.0"?><request><ErrorLevel>1</ErrorLevel><SiteId>0</SiteId>';
my $_xmlError = '<?xml version="1.0" encoding="iso-8859-1"?><eBay><Errors><Error><Code>0</Code><ErrorClass>NetworkError</ErrorClass><SeverityCode>1</SeverityCode><Severity>SeriousError</Severity><ShortMessage>Could not connect to eBay.</ShortMessage><LongMessage>Could not connect to eBay. There was a problem with the client or the network.</LongMessage></Error></Errors></eBay>';

sub new
{
  my ($class  ) = @_;

  my $self = bless
  {
   useragent => LWP::UserAgent->new,
  }, $class;

  $self->getKeys;

  return $self;
}

sub newRequest
{
  my $self = shift;
  my $ref = shift;
  my $result = $_xmlRequest;

  foreach my $key (keys(%$ref))
  {
	$result .= "<$key>" . $ref->{$key} . "</$key>";
  }
  $result .= "</request>";

  return $result;
}

sub request
{
  my $self = shift;
  my $requestXML = shift;
  my $header = HTTP::Headers->new;
  my $requestHash = XML::Smart->new( $requestXML );

  print "callebayapi request\n" if DEBUG;
  print $requestXML, "\n"  if DEBUG;

  $header->push_header( 'X-EBAY-API-COMPATIBILITY-LEVEL' => COMPATIBILITY_LEVEL );
  $header->push_header( 'X-EBAY-API-SESSION-CERTIFICATE' => "$self->{devkey};$self->{appkey};$self->{crtkey}");
  $header->push_header( 'X-EBAY-API-DEV-NAME' => $self->{devkey} );
  $header->push_header( 'X-EBAY-API-APP-NAME' => $self->{appkey} );
  $header->push_header( 'X-EBAY-API-CERT-NAME' => $self->{crtkey} );
  $header->push_header( 'X-EBAY-API-CALL-NAME' => "$requestHash->{request}{Verb}" );
  $header->push_header( 'X-EBAY-API-SITEID' =>  SITE_ID );
  $header->push_header( 'X-EBAY-API-DETAIL-LEVEL' => "$requestHash->{request}{DetailLevel}" );
  $header->push_header( 'Content-Type' => 'text/xml' );
  $header->push_header( 'Content-Length' => length( $requestXML ) );

  # Make the request.
  my $request = HTTP::Request->new( "POST", EBAY_API_URL, $header, $requestXML );
  my $response = $self->{useragent}->request( $request );

  print "NETWORK ERROR\n" if DEBUG && $response->is_error;
  return ( XML::Smart->new( $_xmlError ) ) if $response->is_error;

  print $response->content() if DEBUG;
  return XML::Smart->new( $response->content() );
}


sub test
{
  my $self = shift;
  my $user = shift;
  my $password = shift;

  return $self->getLogoUrl( $user, $password );
}

sub getLogoUrl
{
  my $self = shift;
  my $user = shift;
  my $password = shift;
  my $eBayRequest = $self->newRequest( 
									  {
									   Verb => 'GetLogoURL',
									   Size => 'Small',  
									   RequestUserId => $user,
									   RequestPassword => $password } );
  # Issue the request
  my $eBayResponse = $self->request( $eBayRequest );

  my $url = $eBayResponse->{eBay}{Logo}{URL};

  return $url;
}

sub getKeys
{
  my $self = shift;
  my $ttyInput;
  my $input;
  my $devid;
  my $appid;
  my $crtid;

  # try to open the file
  open( IDS, "c:\\Documents and Settings\\All Users\\ebayrc" ) or die "No keys available.";

  # Read them from the file
  # The file should have the keys in the order
  # developer id
  # application id
  # certificate id
  my $line = 0;
  while ( <IDS> )
  {
	chomp( $_ );
	$devid = $_ if $line == 0;
	$appid = $_ if $line == 1;
	$crtid = $_ if $line == 2;
	$line++;
  }

  # return a list with the keys
  $self->{devkey} = $devid;
  $self->{appkey} = $appid;
  $self->{crtkey} = $crtid;

  return ( $devid, $appid, $crtid );
}

1;
