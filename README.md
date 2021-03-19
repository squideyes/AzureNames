The **AzureName** library and service validate Azure resource names; for Subscriptions, Storage Accounts, LinuxVM's, Network Security Group and more.  

There are two easy-to-use ways to perform name validation using AzureNames; via the NameValidator class and/or the name-validation Service.

The NameValidator class may be found within the (.NET Standard 2.1) AzureNames.Common class library.  To use the NameValidator, do something along the lines of the following:

    var validator = new NameValidator(RuleSetId.Default);

    if (validator.IsValid("mg-fin", NameKind.ManagementGroup))
        Console.Writeline($"The name is GOOD!")
    else
        Console.Writeline($"The name is INVALID!")

The RuleSetId is optional, and if omitted RuleSetId.Default will be in effect.  As of this writing, the "Default" rule-set is the only rule-set extant, but  it would be fairly easy to either create additional rule-sets or expand upon/edit RuleSetId.Default.

To get a better understanding as to what AzureNames does, it'd be good to read  through the **DefaultRuleSet.json** file.  The format should be pretty self explanatory, but in a general sense: rules are name-kind specific, with each name validated by a regular expression ("regex") or according to a "pattern" comprised of one or more "fields," each of which may be validated by it's own "regex" or against a set of preset case-sensitive values.  

Patterns are always defined without hypens, but hyphens are expected to be  
supplied within the validated names.  To take a few examples:

|Name Kind|Pattern or Regex|Sample|
|---|---|---|
|ManagementGroup|{NameCode}{BizUnit}{Environ?}|mg-corp-prd|
|Subscription|{BizUnit}{Environ}{TwoDigit?}|corp-stg-01|
|ResourceGroup|{NameCode}{BizUnit}{MiscText}{Environ}{TwoDigit?}|rg-mktg-sharepoint-prd-01|
|WindowsVM|Azure Naming-Rules Only!!|vmnavigator001|
|LinuxVM|{NameCode}{BizUnit}{MiscText}{Region}{Environ}{ThreeDigit?}|vm-corp-navigator-westus-dev-001|
|StorageAccount|"^[a-z][a-z0-9]{2,23}$|navigatordata001|
|VirtualNetwork|{NameCode}{Environ}{Region}{ThreeDigit?}|vnet-stg-eastus2-001|
|Subnet|{NameCode}{Environ}{Region}{ThreeDigit?}|snet-tst-eastus2-001|
|NetworkInterface|{NameCode}{TwoDigit}{MiscText}{Environ}{ThreeDigit?}|nic-01-dc1-stg-001|
|NetworkSecurityGroup|{NameCode}{MiscText}{ThreeDigit}|nsg-weballow-001|
|PublicIPAddress|{NameCode}{MiscText}{Region}{Environ}{ThreeDigit}|pip-dc1-eastus2-stg-001|

Names validated by regex may be in any format, although in either case each name-kind must conform to it's own set of Azure-specified rules.   

To validate names via the **Name Validator Service**, launch or deploy the "Service" Function App then issue a GET request similar to the following:

* http://localhost:7071/api/mg-mktg/ManagementGroup
* http://localhost:7071/api/**{name to be validated}**/**{NameKind}**

Valid requests will return a confirmation object, with the isValid field set to true or false depending upon the validity of the submitted name:

**{
    "nameKind": "ManagementGroup",
    "name": "mg-mktg",
    "isValid": true,
    "samples": [
        "mg-mktg",
        "mg-fin",
        "mg-corp-prd"
    ]
}**

Invalid requests (i.e. requests with an invlid NameKind) will return a BadRequest object similar to the following: 

**{
    "invalidNameKind": "UnrecognizedNameKind",
    "validNameKinds": [
        "ManagementGroup",
        "Subscription",
        "ResourceGroup",
        "WindowsVM",
        "LinuxVM",
        "StorageAccount",
        "VirtualNetwork",
        "Subnet",
        "NetworkInterface",
        "NetworkSecurityGroup",
        "PublicIPAddress"
    ]
}**


Please feel free to download the source code from GitHub (https://github.com/squideyes/AzureNames) and then to use the project as you see fit.  Better still, fork the project and contribute your own improvements via Pull Requests.  If you need any help in working with the code, please feel free to reach out to me at [louis_berman@epam.com](mailto:louis_berman@epam.com)

Enjoy...

