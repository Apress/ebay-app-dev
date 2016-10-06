#  validuser.pl
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
#    - eBay.SDK.API.ApiSession
#    - eBay.SDK.API.ValidateTestUserRegistrationCall;
#
#  Description
#    This application demonstrates the use of the eBay SDK to
#    validate a test user.

# dependencies
use Win32::OLE;
sub getKeys;

# constants
use constant True => 1;
use constant False => 0;

# create ApiSession Object
print "Creating eBay.SDK.API.ApiSession object...\n";
$apiSession = Win32::OLE->new("eBay.SDK.API.ApiSession");
$apiSession->{'LogCallXml'} = True;
$apiSession->{'Url'} = 
  "https://api.sandbox.ebay.com/ws/api.dll";

# Get the eBay keys
( $apiSession->{'Developer'}, 
  $apiSession->{'Application'}, 
  $apiSession->{'Certificate'} ) = getKeys();

# Find out which account we should validate.
print "Enter eBay User ID of the seller account: ";
chomp($input = <>);
$apiSession->{'RequestUserId'} = $input;

print "Enter password to the seller account: ";
chomp($input = <>);
$apiSession->{'RequestPassword'} = $input;

# Create the API Request
print "Creating ";
print "eBay.SDK.API.ValidateTestUserRegistrationCall ";
print "object...\n";
$api = Win32::OLE->new(
  "eBay.SDK.API.ValidateTestUserRegistrationCall");
$api->{'ApiCall'}->{'ErrorLevel'} = 1;

#Perl doesn't seem to know how to query multiple interfaces
#so we have to use the ApiCall property of the api.
#$api->{'ApiCallSession'} = $apiSession;
$api->{'ApiCall'}->{'ApiCallSession'} = $apiSession;

# Issue the call
print "Issuing ValidateTestUserRegistration() call...\n";
$api->ValidateTestUserRegistration();
$result = Win32::OLE->LastError();

# Show the results
if($result)
{
  print "**Error**: $result!\n";
}
else
{
  print "Congratulations! ";
  print "The user has been validated successfully!\n";
}
print "\n";
<>;

# Fetches the eBay developer keys from either
# a dotfile or from the input line if the dotfile
# isn't availalble.
sub getKeys
{
    # try to open the file
    open( IDS, ".ebayrc" ) or $ttyInput = 1;
    
    # If the open failed prompt the user for each key
    if ( $ttyInput )
    {
	print "Enter API Developer Id: ";
	chomp($input = <>);
	$devid = $input;
	
	print "Enter API Application Id: ";
	chomp($input = <>);
	$appid = $input;

	print "Enter API Certificate: ";
	chomp($input = <>);
	$crtid = $input;
    }
    else
    {
	# Read them from the file
	# The file should have the keys in the order
	# developer id
	# application id
	# certificate id
	$line = 0;
	while ( <IDS> )
	{
	    chomp( $_ );
	    $devid = $_ if $line == 0;
	    $appid = $_ if $line == 1;
	    $crtid = $_ if $line == 2;
	    $line++;
	}
    }
    # return a list with the keys
    return ( $devid, $appid, $crtid );
}
