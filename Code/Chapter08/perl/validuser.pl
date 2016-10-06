=head1 NAME

CallEBayAPI - Provides simple perl script to invoke ebay API calls

=head1 SYNOPSIS

 % validuser.pl

=head1 DESCRIPTION

This application demonstrates the use of the eBay API to
validate a test user.

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

# dependencies
use LWP::UserAgent;
use HTTP::Request;
use HTTP::Headers;
use XML::Simple;
use Data::Dumper;

sub getKeys;

# constants
use constant True => 1;
use constant False => 0;
use constant COMPATIBILITY_LEVEL => 335;
use constant EBAY_API_URL => 'https://api.sandbox.ebay.com/ws/api.dll';
use constant DEBUG => 1;
use constant SITE_ID => '0';
use constant DETAIL_LEVEL => '0';
use constant VERB => 'ValidateTestUserRegistration';

# variables
my $input; # user input
my $siteid = SITE_ID;
my $detaillevel = DETAIL_LEVEL;
my $verb = VERB;
my $xml = <<TEMPLATE;
<?xml version="1.0"?>
<request>
  <RequestUserId>^userid^</RequestUserId>
  <RequestPassword>^password^</RequestPassword>
  <ErrorLevel>1</ErrorLevel>
  <DetailLevel>^detaillevel^</DetailLevel>
  <Verb>^verb^</Verb>
  <SiteId>^siteid^</SiteId>
</request>
TEMPLATE


# Create the user agent object
print "Creating user agent object...\n";
my $useragent = LWP::UserAgent->new;

# Get the eBay keys
my ( $devKey, $appKey, $crtKey ) = getKeys();

# Find out which account we should validate.
print "Enter eBay User ID of the seller account: ";
chomp($input = <>);
my $userid = $input;

print "Enter password to the seller account: ";
chomp($input = <>);
my $password = $input;

# Create the request XML.
$xml =~ s/\^userid\^/$userid/g;
$xml =~ s/\^password\^/$password/g;
$xml =~ s/\^detaillevel\^/$detaillevel/g;
$xml =~ s/\^verb\^/$verb/g;
$xml =~ s/\^siteid\^/$siteid/g;


# Create the API Request HTTP Headers
my $header = HTTP::Headers->new;
$header->push_header( 'X-EBAY-API-COMPATIBILITY-LEVEL' => COMPATIBILITY_LEVEL );
$header->push_header( 'X-EBAY-API-SESSION-CERTIFICATE' => "$devKey;$appKey;$crtKey");
$header->push_header( 'X-EBAY-API-DEV-NAME' => $devKey );
$header->push_header( 'X-EBAY-API-APP-NAME' => $appKey );
$header->push_header( 'X-EBAY-API-CERT-NAME' => $crtKey );
$header->push_header( 'X-EBAY-API-CALL-NAME' => VERB );
$header->push_header( 'X-EBAY-API-SITEID' =>  SITE_ID );
$header->push_header( 'X-EBAY-API-DETAIL-LEVEL' => SITE_ID );
$header->push_header( 'Content-Type' => 'text/xml' );
$header->push_header( 'Content-Length' => length( $xml ) );

print $xml, "\n" if DEBUG;

# Create the request
my $request = HTTP::Request->new("POST", EBAY_API_URL, $header, $xml);

# Issue the call
my $response = $useragent->request($request);

# handle transport errors first.
die "Network error contacting eBay." if $response->is_error;

print $response->content() if DEBUG;
# Parse the result.
my $result = XMLin( $response->content() );
print Dumper( $result ) if DEBUG;

print "\n";

# Show the results
if( $result->{Errors} )
{
  print "**Error**:", $result->{Errors}->{Error}->{LongMessage}, "\n";
}
else
{
  print "Congratulations! ";
  print "The user has been validated successfully!\n";
}
print "\n";
<>;

# Fetches the eBay developer keys from either
# a dotfile or from the input line if the dotfile
# isn't availalble.
sub getKeys
{
  my $ttyInput;
  my $input;
  my $devid, $appid, $crtid;

  # try to open the file
  open( IDS, "c:\\Documents and Settings\\All Users\\ebayrc" ) or $ttyInput = 1;

  # If the open failed prompt the user for each key
  if ( $ttyInput )
  {
	print "Enter API Developer Id: ";
	chomp($input = <>);
	my $devid = $input;

    print "Enter API Application Id: ";
    chomp($input = <>);
	$appid = $input;

	print "Enter API Certificate: ";
	chomp($input = <>);
	$crtid = $input;
  }
  else
  {
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
  }
  # return a list with the keys
  return ( $devid, $appid, $crtid );
}
