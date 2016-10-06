=head1 NAME

Categories.pl - Demonstrates the Integration Library's
Categories interfaces.

=head1 DESCRIPTION

This package provides an example of how to get categories
using the Integration Library.

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
use Tk;
use Tk::Tree;
use Tk::DialogBox;

my $application = eBaySampleApplication->new;
my $mw = $application->{MAINWINDOW};

# Initialize the Integration Library
$application->InitIntegrationLibrary();

# You must set this to a valid value
$application->AppUserId( 1 );

# Create the three frames
my $rootFrame = $mw->Frame;
my $inputFrame = $rootFrame->Frame;
my $outputFrame = $rootFrame->Frame;

# Fill the input frame
my $button = $inputFrame->Button( -text => "Categories",
								   -command => \&Categories_Click );

Tk::grid( $button );

# Fill the output frame
$tree = $outputFrame->ScrlTree( qw( separator /
                                -width 35 -height 25
                                -scrollbars osoe))->pack;

$tree->configure( -opencmd => sub { opencmd( $tree, @_ ); },
				  -closecmd => sub { closecmd( $tree, @_ ); } );

# Pack the frames
$inputFrame->pack;
$outputFrame->pack;

# Get the categories from the database
$categories = $application->{ENVIRONMENT}->LoadAllCategories( $application->SiteIDEnum_US );
  my $result = Win32::OLE->LastError();

  if($result)
  {
#	$mw->messageBox(
#	  -message => 'Could not connect to eBay.',
#       -title => 'Error' );
	print $result;
	return;
  }
# Seed the top level of the tree.
for ( $i = 0; $i < $categories->ItemCount(); $i ++ )
{
  if ( $categories->ItemAt($i)->{CategoryName} && 
	   $categories->ItemAt($i)->{CategoryId} == $categories->ItemAt($i)->{CategoryParentId} )
  {
	$tree->add( $categories->ItemAt($i)->{CategoryId},
				-text => $categories->ItemAt($i)->{CategoryName} );
    $tree->setmode( $categories->ItemAt($i)->{CategoryId},
					-d $categories->ItemAt($i)->{CategoryLeaf} ? "none" : "open" );
  }
}

$application->Gui( $rootFrame );

#
# This is specific to using the integration manager only.
#


$application->Main;

#
#
# Below lies thiing that don't work.

# Adds a hierarchical item to the indicated tree.
sub additem
{
    my( $tree, $path, $name ) = @_;
    $tree->add( $path, -text => $name );
}

sub opencmd
{
  my( $tree, $path ) = @_;
  my  $currentId = (split( "/", $path ))[-1];

  # Only do the work once...
  #  return if $tree->info('children', $path );

  $application->Busy( 1 );
  # For each category, if it has a name and
  # its parent is the current ID and it's not its parent, add it.
  for ( $i = 0; $i < $categories->ItemCount(); $i ++ )
  {
	if ( $categories->ItemAt($i)->{CategoryName} &&
		 $categories->ItemAt($i)->{CategoryParentId} == $currentId &&
		 $categories->ItemAt($i)->{CategoryId} !=  $categories->ItemAt($i)->{CategoryParentId} )
	{
	  my $id = $categories->ItemAt($i)->{CategoryId};
	  my $name = $categories->ItemAt($i)->{CategoryName};
	  my $leaf = $categories->ItemAt($i)->{LeafCategory};
	  additem( $tree, "$path/$id", $name );
	  $tree->setmode( "$path/$id", !$leaf ? "open" : "none" );
	}
  }
  $application->Busy( 0 );
}

# Close a hierarchical list entry without caching contents.
sub closecmd
{
  # We don't cache categories 
  my( $tree, $dir ) = @_;
  $tree->delete( 'offsprings', $dir );
}

#
# Update the categories in the database.
#
sub Categories_Click
{
  $application->Busy( 1 );

  $application->{SESSION}->{EBaySynchronizer}->SynchronizeCategories( $application->SiteIDEnum_US, 0);
  my $result = Win32::OLE->LastError();
  if($result)
  {
#	$mw->messageBox(
#	  -message => 'Could not connect to eBay.',
#       -title => 'Error' );
	print $result;
	return;
  }

  $application->Busy( 0 );
}
