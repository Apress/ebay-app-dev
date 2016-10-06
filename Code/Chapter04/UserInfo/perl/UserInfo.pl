=head1 NAME

userinfo.pl - Demonstrates the GetUserCall API

=head1 DESCRIPTION

This package provides an example of how to get user information
using the GetUserCall API and the User class.

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



require eBaySampleApplication;
use Tk;
use Tk::ROText;

my $application = eBaySampleApplication->new;
my $mw = $application->{MAINWINDOW};

$userid = $application->{APISESSION}->{RequestUserId};

# Create the three frames
my $rootFrame = $mw->Frame;
my $inputFrame = $rootFrame->Frame;
my $outputFrame = $rootFrame->Frame;

# Fill the input frame
my $label1 = $inputFrame->Label( -text => "eBay User ID" );
my $entry1 = $inputFrame->Entry( -textvariable => \$userid );
my $label2 = $inputFrame->Label( -text => "DetailLevel" );
my $detaillevel = $inputFrame->Scrolled( "Listbox",
										-height => 1,
									    -scrollbars => "oe" );
$detaillevel->insert( 'end', qw( 0 2 4 6 8 10 ) );
$detaillevel->selectionSet(0);

my $button1 = $inputFrame->Button( -text => "Find",
								   -command => \&find_Click );
Tk::grid( $label1, $entry1, $label2, $detaillevel, $button1 );

# Fill the output frame
$resultText = $outputFrame->Scrolled( "ROText",
									  -height => 8,
									  -width => 64,
									  -scrollbars => "oe" )->pack;

# Pack the frames
$inputFrame->pack;
$outputFrame->pack;

$application->Gui( $rootFrame );

$application->Main;

#
# do the eBay thing
#
sub find_Click
{
  $application->Busy( 1 );

  my $api = Win32::OLE->new( "eBay.SDK.API.GetUserCall" );
  $api->{ApiCall}->{ApiCallSession} = $application->{APISESSION};
  $api->{ApiCall}->{ErrorLevel} = 1;
  $api->{CompatibilityLevel} = 319;

  $api->{UserId} = $userid;
  my ( $detaillevelidx ) = $detaillevel->curselection();
  $api->{DetailLevel} = $detaillevelidx * 2;

  my $theUser = $api->GetUser();
  my $result = Win32::OLE->LastError();

  # Show the results
  if( $theUser )
  {
	$result = $theUser->{UserId} . "\n" .
	  $theUser->{Email} . "\n";
	$result .= $theUser->{PaymentAddress} . "\n"
	  if $theUser->{PaymentAddress};
	$result .= "Payment address not permitted.\n"
	  if !$theUser->{PaymentAddress};
	$result .= $theUser->{RegistrationAddress} . "\n"
	  if $theUser->{RegistrationAddress};
	$result .= "Registration address not permitted.\n"
	  if !$theUser->{RegistrationAddress};
	$result .= "The user is ";
	$result .= "not "  if !$theUser->{IdVerified};
	$result .= "verified\n";
	$result .= "The user";
	$result .= " has " if ( $theUser->{AboutMe} );
	$result .= " doesn't have " if ( !$theUser->{AboutMe} );
	$result .= 	"an AboutMe page.\n";
  }

  $resultText->delete( "1.0", "end" );
  $resultText->insert( "end", $result );
  $application->Busy( 0 );
}
