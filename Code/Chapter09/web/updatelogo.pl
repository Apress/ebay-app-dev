=head1 NAME

updatelogo.pl - Updates PERL script which returns a redirect
to obtai the eBay Marketplace logo.

=head1 SYNOPSYS
  updatelogo.pl > ebaylogo.pl


=head1 DESCRIPTION
Run updatelogo daily manually or via crontab to update the
eBay logo for this site.


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
use CallEBayAPI;
use strict;

my $filename = shift;

my $eBayInterface = CallEBayAPI->new;

my $url = $eBayInterface->getLogoUrl( "", "" );

open FILE, ">$filename";
print FILE "use CGI;\n";
print FILE "\$query = new CGI;\n";
print FILE "print \$query->redirect(\"$url\");\n";
close FILE;
