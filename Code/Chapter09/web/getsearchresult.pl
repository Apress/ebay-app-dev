use CallEBayAPI;
use Data::Dumper;
use CGI qw(:standard);
use strict;
sub getKeys;

# Get the arguments for the search
my $query = new CGI;
my $userid = param( "userid" );
my $password = param( "password" );
my $querydata = param( "querydata" );


#my $userid = "sandy2718";
#my $password ="ebaytest";
#my $querydata = "phone";


# Create the eBay interface
my $eBayInterface = CallEBayAPI->new;

# output the header first
print header();
print start_html("eBay Search Results");
print h1( "<img src=\"logo.pl\">eBay Search Results");

# now perform the request
my $skip = 0;
my $done = 0;
my $request;
my $response;
my $i;

while( !$done )
{
  $request = $eBayInterface->newRequest( {
										  Verb => 'GetSearchResults',
										  DetailLevel => '0',
										  RequestUserId => $userid,
										  RequestPassword => $password,
										  Active => '1',
										  SearchInDescription => '1',
										  ItemTypeFilter => '3',
										  Order => 'MetaEndSort', 
										  Query => $querydata,
										  SearchType => '0',
										  MaxResults => '3',
										  SiteIdFilterType => '2',
										  Skip => $skip } );
  $response = $eBayInterface->request( $request );

  if ( $response->{eBay}->{Errors}->{Error}->{LongMessage} )
  {
	if ( $skip > 0 )
	{
	  # end the table first.
	  print "</table>\n";
	}
	print p("An error occurred. The eBay service returned: " .
			i( blockquote( $response->{eBay}->{Errors}->{Error}->{LongMessage} ) ) );
	last;
  }

  # If we are just starting, print the total number of items.
  if ( $skip == 0 && $response->{eBay}->{Search}->{GrandTotal} == 0  )
  {
	print p( "No items matched your query." );
  }
  if ( $skip == 0 && $response->{eBay}->{Search}->{GrandTotal} != 0  )
  {
	print p( "There were $response->{eBay}->{Search}->{GrandTotal} items matching your query."), "\n";
	print "<table>\n";
	print "<tr>";
	print td( "Item #" );
	print td( "Title" );
	print td( "Number of Bids" );
	print td( "Current Price" );
	print td( "Start Time" );
	print td( "End Time" );
	print "</tr>\n";
  }

  $i = 0;
  # print the table row for each item returned.
  my @items = @{$response->{eBay}->{Search}->{Items}->{Item}};
  foreach my $item (@items)
  {
	if ( $item->{Id} )
	{
	  print "<tr>";
	  print td( "<a href=\"$item->{Link}\" target=\"_new\">" .
				$item->{Id} . "</a>" );
	  print td( $item->{Title} );
	  print td( $item->{BidCount} );
	  print td( "\$" . $item->{CurrentPrice} );
	  print td( $item->{StartTime} );
	  print td( $item->{EndTime} );
	  print "</tr>\n";
	  $i++;
	}
  }
  $skip += $response->{eBay}->{Search}->{Count};
  # and if we're done, exit the loop
  $done = !$response->{eBay}->{Search}->{HasMoreItems};
  if ( $done )
  {
	print "</table>";
  }
}
OUTOFLOOP:

# output a note about the app
print hr();
print p( "This web page is part of the book ", 
		 i("Developing Applications with eBay"),
		 "written by Ray Rischpater and available ",
		 "from ",
		 "<a href=\"http://www.apress.com\">Apress, L.P.</a>" );
# end the HTML
print end_html();

