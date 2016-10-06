#  .pl
#
#  Created by Ray Rischpater on 12 July 2003
#  Copyright (c) 2003 by the author. All rights reserved.
#  This file is provided without any warranty, express or
#  implied, including but not limited to fitness of purpose.
#  You may use this file so long as the copyright attribution
#  remains.
#
#  This file is part of the book "Developing Applications with
#  eBay" written by Ray Rischpater and available from
#  Apress, LP.
#
#  Dependencies
#    - eBay SDK
#    - ActivePerl
#
#  Demonstrates
#
#  Description

# Dependencies
use Tk;
use Tk::Tree;
use Tk::DialogBox;
use Win32::OLE;

$apiSession =
  Win32::OLE->new( "eBay.SDK.API.ApiSession" );
$apiSession->{'LogCallXml'} = True;
$apiSession->{'Url'} =
  "https://api.sandbox.ebay.com/ws/api.dll";
$apiSession->{'Log'} = Win32::OLE->new( "ebay.SDK.LogFile" );
$apiSession->{'Log'}->Open( "c:\\ebaylog.txt" );


# Get the eBay keys
( $apiSession->{'Developer'},
  $apiSession->{'Application'},
  $apiSession->{'Certificate'},
  $apiSession->{'RequestUserId'},
  $apiSession->{'RequestPassword'}
 ) = loadKeys();

# Initialize our GUI: a main window and a scrolling tree.
$mw = MainWindow->new;
$tree = $mw->ScrlTree( qw( separator /
                              -width 35 -height 25
                              -scrollbars osoe));
$tree->configure( -opencmd => sub { opencmd( $tree, @_ ); },
				  -closecmd => sub { closecmd( $tree, @_ ); } );

# create the GetCategoriesCall object
$api = Win32::OLE->new(
  "eBay.SDK.API.GetCategoriesCall");
$api->{'ApiCall'}->{'ApiCallSession'} = $apiSession;
$api->{'ApiCall'}->{'ErrorLevel'} = 1;
$api->{'DetailLevel'} = 1;

# Show the GUI.
guiInit( $tree );

my $categories =  GetCategories( 0, 2 );
for ( $i = 0; $i < $categories->ItemCount(); $i ++ )
{
  if ( $categories->ItemAt($i)->{CategoryName} )
  {
	$tree->add( $categories->ItemAt($i)->{CategoryId},
				-text => $categories->ItemAt($i)->{CategoryName} );
    $tree->setmode( $categories->ItemAt($i)->{CategoryId},
					-d $categories->ItemAt($i)->{CategoryLeaf} ? "none" : "open" );
  }
}

## $categories is of type eBay.SDK.Model.ICategoryCollection
# Show the results

MainLoop;

# Gets the child categories down to a specific depth from eBay
sub GetCategories
{
  my( $parent, $depth ) = @_;

  # Set up the API we'll use
  $api->{'CategoryParent'} = 0;
  $api->{'LevelLimit'} = 1;

  my $categories = $api->GetCategories();

  $result = Win32::OLE->LastError();
  if($result)
  {
	$mw->messageBox( -title => "Error",
					 -message => "GetCategories call failed: $result",
					 -icon => "error" );
  }
  return $categories;
}


# Adds a hierarchical item to the indicated tree.
sub additem
{
    my( $tree, $dir ) = @_;

    my $text = $dir;
	# Get the last word of $dir split by '/'
    $text = (split( "/", $dir ))[-1] unless $text eq "/";

    $tree->add( $dir, -text => $text );
    $tree->setmode( $dir, -d $dir ? "open" : "none" );
}

sub opencmd
{
    my( $tree, $dir ) = @_;

	print $dir, "\n" ;

	# Add code here to request categories below current category
	# if $dir is /, need to get baseline catgegories.
	# otherwise, get categories contained by $dir

}

# Close a hierarchical list entry without caching contents.
sub closecmd
{
  # We don't cache categories --- with over ten thousand,
  # we'd eat a ridiculous amount of memory.
  my( $tree, $dir ) = @_;
  $tree->delete( 'offsprings', $dir );
}


#
# General applicaiton GUI configuration
#
sub guiInit
{
  my ( $mainItem ) = @_;

  my $status = $mw->Label( -text => '' );
  my $config = $mw->Button( -text => 'Settings',
							-command => \&guiConfig );
  $mainItem->pack();
  $status->pack( side => 'left' );
  $config->pack( side => 'bottom' ) ;

  return ( $status, $config );
}


sub guiConfig
{
  my $configDialog = $mw->DialogBox(-title => "Settings", -buttons => [ "OK", "Cancel" ] );
  $configDialog->add( 'Label', ( -text => "eBay Developer ID:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable => \$apiSession->{'Developer'} ) )->pack();
  $configDialog->add( 'Label', ( -text => "eBay Application ID:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable => \$apiSession->{'Application'} ) )->pack();
  $configDialog->add( 'Label', ( -text => "eBay Certificate ID:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable => \$apiSession->{'Certificate'} ) )->pack();
  $configDialog->add( 'Label', ( -text => "User:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable => \$apiSession->{'RequestUserId'} ) )->pack();
  $configDialog->add( 'Label', ( -text => "Password:" ) )->pack();
  $configDialog->add( 'Entry', ( -textvariable => \$apiSession->{'RequestPassword'} ) )->pack();

  my $result = $configDialog->Show();
  if ( $result == "OK" )
  {
	saveKeys( $apiSession->{'Developer'},
			  $apiSession->{'Application'},
			  $apiSession->{'Certificate'},
			  $apiSession->{'RequestUserId'},
			  $apiSession->{'RequestPassword'},
		   );
  }
}


sub saveKeys
{
  my ( $devid, $appid, $crtid, $user, $password  ) = @_;
  open( IDS, ">c:\\Documents and Settings\\All Users\\ebayrc" ) or
	$mw->messageBox( -title => "Error",
					 -message => "Could not save settings."
					 -icon => "error" );
  print IDS "$devid\n";
  print IDS "$appid\n";
  print IDS "$crtid\n";
  print IDS "$user\n";
  print IDS "$password\n";
  $apiSession->{'Developer'} = $devid;
  $apiSession->{'Application'} = $appid;
  $apiSession->{'Certificate'} = $crtid;
  $apiSession->{'RequestUserId'} = $user;
  $apiSession->{'RequestPassword'} = $password;
  close IDS;
}

sub loadKeys
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
	  while ( <IDS> )
	  {
	    chomp( $_ );
	    $devid = $_ if $line == 0;
	    $appid = $_ if $line == 1;
	    $crtid = $_ if $line == 2;
		$user = $_ if $line == 3;
		$password = $_ if $line == 4;
	    $line++;
	  }
	  close IDS;
    }
    # Return a list with the keys
    return ( $devid, $appid, $crtid, $user, $password );
}


__END__
=head1 NAME

categories - Provides a hierarchical view of eBay's categories

=head1 SYNOPSIS

B<file>


=head1 DESCRIPTION

This application provides a hierarchical tree view (using Tk::Tree)
into the eBay category structure.

It demonstrates the following eBay API's:



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
