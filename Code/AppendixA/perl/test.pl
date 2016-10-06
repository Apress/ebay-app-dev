=head1 NAME

test.pl - Sample to accompany eBaySampleApplication.pm

=head1 DESCRIPTION

This package provides a simple invocation of the
eBaySampleApplication to demonstrate a sample
application for the eBay service.

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
my $application = eBaySampleApplication->new;
my $ui = $application->{MAINWINDOW}->Label( -text => 'Hello World' );
$application->Gui( $ui );

$application->Main;
