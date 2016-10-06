=head1 NAME

TestCallEBayAPI - Provides simple perl script to test the CallEBayAPI module.

=head1 SYNOPSIS
  % TestCallEBayAPI.pl
  The eBay Logo URL is http://www.eBay.com/something.gif


=head1 DESCRIPTION

This file invokes the CallEBayAPI's test method to
verify the basic functionality of the CallEBayAPI package.

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

# Create the eBay interface
my $eBayInterface = CallEBayAPI->new;

print "The eBay Logo URL is ";

my $url = $eBayInterface->test( "login", "password" );

print $url if $url;
print "not available because the test failed" if !$url;

