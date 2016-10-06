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
use Win32::OLE;
use Tk;
use Tk::ROText;
use Date;
use Data::Dumper;

my $application = eBaySampleApplication->new;
my $mw = $application->{MAINWINDOW};

$userid = $application->{APISESSION}->{RequestUserId};
$page = "0";

# Create the three frames
my $rootFrame = $mw->Frame;
my $inputFrame = $rootFrame->Frame;
my $outputFrame = $rootFrame->Frame;

# Fill the input frame
my $label1 = $inputFrame->Label( -text => "eBay User ID" );
my $entry1 = $inputFrame->Label( -textvariable => \$userid,
							     -relief => "sunken" );
my $label2 = $inputFrame->Label( -text => "View by" );
my $viewtype = $inputFrame->Listbox( -height => 2 );
my @items = qw( Period Invoice );
foreach (@items)
{
  $viewtype->insert( 'end', $_ );
}
$viewtype->selectionSet(0);
my $entry2 = $inputFrame->Entry( -textvariable => \$viewby,
							     -width => 3 );
my $label3 = $inputFrame->Label( -text => "Page" );
my $pagenumber = $inputFrame->Scrolled( "Listbox", 
										-height => 2,
									    -scrollbars => "oe" );
$pagenumber->insert( 'end', "1" );
Tk::grid( $label1, $entry1,
		  $label2, $viewtype,
		  $label3, $pagenumber ) ;

my $label4 = $inputFrame->Label( -text => "Start" );
$startdate = $inputFrame->Date( -datefmt => '%2m/%2d/%4y',
								-fields => 'date',
								-value => 'now' );
my $label5 = $inputFrame->Label( -text => "End" );
$enddate = $inputFrame->Date( -datefmt => '%2m/%2d/%4y',
							  -fields => 'date',
							  -value => 'now' );
my $button = $inputFrame->Button( -text => "Find",
								   -command => \&find_Click );
Tk::grid( $label4, $startdate, $label5, $enddate, $button );
# Fill the output frame
my $resultText = $outputFrame->Scrolled( "ROText", 
										 -height => 6,
										 -width => 64,
									     -scrollbars => "oe" )->pack;

# Pack the frames
$inputFrame->pack;
$outputFrame->pack;

$application->Gui( $rootFrame );

$application->Main;

#
# Do the eBay thing
#
sub find_Click
{
  $application->Busy( 1 );

  my $api = Win32::OLE->new( "eBay.SDK.API.GetAccountCall" );
  $api->{CompatibilityLevel} = 319;
  $api->{ApiCall}->{ApiCallSession} = $application->{APISESSION};
  $api->{ApiCall}->{ErrorLevel} = 1;

  ($api->{PageNumber}) = $pagenumber->curselection();
  $api->{PageNumber} = 1 if !$api->{PageNumber};
  ($api->{ViewType}) = $viewtype->curselection();
  $api->{ViewType} = 0 if !$api->{ViewType};

  if ( $api->{ViewType} == 0 )
  {
	my $popt = Win32::OLE->new( "eBay.SDK.Model.Account.PeriodViewSettings" );
	my $dr = Win32::OLE->new( "eBay.SDK.API.DateRangeImpl" );
	$dr->{BeginDate} = $startdate->get( "%Y-%m-%d" );
	$dr->{EndDate} = $enddate->get( "%Y-%m-%d" );
	$popt->{DateRange} = $dr;
	$api->{PeriodViewOption} = $popt;
  }

  if ( $api->{ViewType} == 1 )
  {
	my $ivopt = Win32::OLE->new( "eBay.SDK.Model.Account.InvoiceViewSettings" );
	$ivopt->{InvoiceMonth} = $startdate->get( "%m" );
	$ivopt->{InvoiceYear} = $startdate->get( "%y" );
	$api->{InvoiceViewOption} = $ivopt;
  }

  my $theAccount = $api->GetAccount();
  my $result = Win32::OLE->LastError();

  # Show the results
  if( $theAccount )
  {
	$result = "Account: " . $theAccount->{Id} . "\n";
	if ( $theAccount->{InvoiceView} )
	{
	  $result .= "Amount Due:\t" . $theAccount->{InvoiceView}->{InvoiceBalance} . "\n";
	  $result .= "Invoice Date:\t" . $theAccount->{InvoiceView}->{InvoiceDate}}->Date("MM/dd/yyyy") . "\n";
	}
	if ( $theAccount->{PeriodView} )
	{

		$result .= "Last Invoice Date:\t" . 
		  $theAccount->{PeriodView}->{LastInvoiceDate}->Date("MM/dd/yyyy") . "\n";
		$result .= "Last Invoice Amount:\t" .
		  $theAccount->{PeriodView}->{LastInvoiceAmount} . "\n";
		$result .= "Last Payment Date:\t" .
		  $theAccount->{PeriodView}->{LastPaymentDate}->Date("MM/dd/yyyy") . "\n";
		$result .= "Last Payment Amount:\t" .
		  $theAccount->{PeriodView}->{LastAmountPaid} . "\n";
		$result .= "Current Balance:\t" .
		  $theAccount->{PeriodView}->{CurrentBalance} . "\n";
	}

	# Enumerate each of the account activities
	$result .= "\nActivity History\n";
	my $i;
	for ( $i = 0; $i < $theAccount->{Activities}->ItemCount(); $i++ )
	{
	  my $activity = $theAccount->{Activities}->ItemAt($i);
	  $result .= $activity->{Date}->Date("MM/dd/yyyy") . "\t";
	  $result .= $activity->{Id} . "\t";
	  $result .= $activity->{Credit} . "\t" if $activity->{Credit};
  	  $result .= "(" . $activity->{Debit} . ")" . "\t" if $activity->{Debit};
	  $result .= "\n";
	}

	# Update the pagenumber menu with the list pages
	$pagenumber->delete( 0, "end" );
	for ( $i = 1; $i <= $api->{TotalPages}; $i++ )
    {
	  $pagenumber->insert( "end", "$i" );
	}
	
	$resultText->delete( "1.0", "end" );
	$resultText->insert( "end", $result );
  }
  $application->Busy( 0 );
}
