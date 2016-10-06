=head1 NAME

eBaySampleApplication - Provides simple Perl/tk application for use with eBay

=head1 SYNOPSIS

  require eBaySampleApplication;
  my $application = eBaySampleApplication->new;
  my $ui = $application->{MAINWINDOW}->Label( -text => 'Hello World' );
  $application->gui( $ui );
  $application->main;

=head1 DESCRIPTION

This package provides a wrapper application to demonstrate
application development with the eBay service.

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


package eBaySampleApplication;

# Dependencies
use Tk::DialogBox;
use Win32::OLE;
my $app;
my $statustext;
#
# Constructor for sample application
#
sub new
{
  my $class = shift;

  return if $app;

  $app = bless {}, $class;

  $app->{MAINWINDOW} = MainWindow->new;
  $app->{APISESSION} = Win32::OLE->new("eBay.SDK.API.ApiSession");
  $app->{APISESSION}->{LogCallXml} = True;
  $app->{APISESSION}->{Url} = "https://api.sandbox.ebay.com/ws/api.dll";
  $app->{APISESSION}->{Log} = Win32::OLE->new( "ebay.SDK.LogFile" );
  $app->{APISESSION}->{Log}->Open("c:\\ebaylog.txt" );

  # load our settings and keys
  $app->_loadKeys;

  return $app;
}

#
# Creates GUI, invokes Tcl/Tk MainLoop;
#
sub Main
{
  my $self = shift;
  my $config = $self->{MAINWINDOW}->Button( -text => 'Settings',
											-command => \&_configDialog_ShowDialog );
  $self->{STATUS} = $self->{MAINWINDOW}->Label( -textvariable => \$self->{STATUSTEXT},
											    -width => 35 );

  $self->{GUI}->pack( ) if $self->{GUI};

  $self->{STATUS}->pack( -side => 'left' );
  $config->pack( -side => 'bottom', -anchor=> "se" ) ;

  &Tk::MainLoop;
}


#
# General application GUI configuration
#
sub Gui
{
  my $self = shift;
  $self->{GUI} = shift if ( @_ );
  return $self->{GUI};
}

#
# Set the status text
#
sub StatusText
{
  my $self = shift;
  $self->{STATUSTEXT} = shift if ( @_ );
  return $self->{STATUSTEXT};
}

# 
# Set the app to look busy
#
sub Busy
{
  my $self = shift;
  $self->{BUSY} = shift if ( @_ );
  if ( $self->{BUSY} )
  {
	$self->{MAINWINDOW}->Busy( -recurse => 1 );
  }
  else
  {
	$self->{MAINWINDOW}->Unbusy;
  }
  return $self->{BUSY};
}

#
# Handle Configuration Dialog
#
sub _configDialog_ShowDialog
{
  my $configDialog = $app->{MAINWINDOW}->DialogBox(-title => "Settings",
												   -buttons => [ "OK", "Cancel" ] );

  $configDialog->add( 'Label', ( -text => "eBay Developer ID:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable =>
								 \$app->{APISESSION}->{Developer} ) )->pack();
  $configDialog->add( 'Label', ( -text => "eBay Application ID:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable =>
								 \$app->{APISESSION}->{Application} ) )->pack();
  $configDialog->add( 'Label', ( -text => "eBay Certificate ID:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable =>
								 \$app->{APISESSION}->{Certificate} ) )->pack();
  $configDialog->add( 'Label', ( -text => "User:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable =>
								 \$app->{APISESSION}->{RequestUserId} ) )->pack();
  $configDialog->add( 'Label', ( -text => "Password:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable =>
								 \$app->{APISESSION}->{RequestPassword} ) )->pack();

  my $result = $configDialog->Show();
  if ( $result == "OK" )
  {
	$app->_saveKeys;
  }

}

#
# Save the access keys
#
sub _saveKeys
{
  open( IDS, ">c:\\Documents and Settings\\All Users\\ebayrc" ) or
	$app->{MAINWINDOW}->messageBox( -title => "Error",
									-message => "Could not save settings."
									-icon => "error" );
  print IDS $app->{APISESSION}->{Developer}, "\n";
  print IDS $app->{APISESSION}->{Application}, "\n";
  print IDS $app->{APISESSION}->{Certificate}, "\n";
  print IDS $app->{APISESSION}->{RequestUserId}, "\n";
  print IDS $app->{APISESSION}->{RequestPassword}, "\n";
  print IDS	$app->{DATABASEINFO}, "\n";
  close IDS;
}

#
# load the access keys
#
sub _loadKeys
{
    # try to open the file
    open( IDS, "c:\\Documents and Settings\\All Users\\ebayrc" ) or $fail = 1;

	if ($fail)
    {
	  $devid = "developerid";
	  $appid = "applicationid";
	  $crtid = "certificateid";
	  $user = "user";
	  $password = "password";
    }
    else
    {
	  # Read them from the file
	  # The file should have the keys in the order
	  # developer id
	  # application id
	  # certificate id
	  # user
	  # password
	  $line = 0;
	  while (n <IDS> )
	  {
	    chomp( $_ );
		$app->{APISESSION}->{Developer} = $_ if $line == 0;
		$app->{APISESSION}->{Application} = $_ if $line == 1;
		$app->{APISESSION}->{Certificate} = $_ if $line == 2;
		$app->{APISESSION}->{RequestUserId} = $_ if $line == 3;
		$app->{APISESSION}->{RequestPassword} = $_ if $line == 4;
		$app->{DATABASEINFO} = $_ if $line == 5;
	    $line++;
	  }
	  close IDS;
    }
}

1;


