use XML::Simple;
use Data::Dumper;

my $xml = <<TEMPLATE;
<?xml version="1.0"?>
<request>
  <RequestUserId>^userid^</RequestUserId>
  <RequestPassword>^password^</RequestPassword>
  <DetailLevel>1</DetailLevel>
  <ErrorLevel>1</ErrorLevel>
  <SiteId>0</SiteId>
  <Verb>GetCategories</Verb>
  <CategoryParent>^parent^</CategoryParent>
  <LevelLimit>^limit^</LevelLimit>
</request>
TEMPLATE

# $xml contains the desired XML.
$xml =~ s/\^userid\^/sandy2718/g;
$xml =~ s/\^password\^/magic/g;
$xml =~ s/\^parent\^/2/g;
$xml =~ s/\^limit\^/1/g;

my $result = XMLin( $xml );

print Dumper( $result );


