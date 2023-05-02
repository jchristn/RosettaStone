![alt tag](https://raw.githubusercontent.com/jchristn/RosettaStone/main/assets/rosettastone.png)

# RosettaStone

Rosetta Stone is an application that provides a centralized repository of metadata about vendors and CODECs involved in the creation of DNA data storage archives.  With Rosetta Stone, an archive reader could amplify and sequence what is known as "sector 0" to discern lookup values that, when presented to this application, returns to the user metadata about the vendor that wrote the archive and the CODEC used to write sector 1.

## New in v1.0.x

- Initial release

## APIs

Refer to the Postman collection in the repository for a quickstart on the Rosetta Stone APIs.

### Get All Vendors
```
GET /v1.0/vendor

Success: 200/OK
[
	{
	    "GUID": "[guid]",
	    "Key": "[key]",
	    "Name": "[name]",
	    "ContactInformation": "[contact]",
	    "CreatedUtc": "2023-04-26T19:16:28.382196",
	    "LastModifiedUtc": "2023-04-26T19:16:28.382196"
	},
	...
]
```

### Get Vendor by Key
```
GET /v1.0/vendor/[key]

Success: 200/OK
{
    "GUID": "[guid]",
    "Key": "[key]",
    "Name": "[name]",
    "ContactInformation": "[contact]",
    "CreatedUtc": "2023-04-26T19:16:28.382196",
    "LastModifiedUtc": "2023-04-26T19:16:28.382196"
}

Failure: 404
{
    "Message": "The requested object was not found."
}
```

### Find Closest Vendor Match
```
GET /v1.0/vendor/match/[key]

Success: 200/OK
{
    "GUID": "[guid]",
    "Key": "[key]",
    "Name": "[name]",
    "ContactInformation": "[contact]",
    "CreatedUtc": "2023-04-26T19:16:28.382196",
    "LastModifiedUtc": "2023-04-26T19:16:28.382196",
    "EditDistance": 2
}
```

### Find Closest Vendor Matches
```
GET /v1.0/vendor/match/[key]?results=10

Success: 200/OK
[
	{
	    "GUID": "[guid]",
	    "Key": "[key]",
	    "Name": "[name]",
	    "ContactInformation": "[contact]",
	    "CreatedUtc": "2023-04-26T19:16:28.382196",
	    "LastModifiedUtc": "2023-04-26T19:16:28.382196",
	    "EditDistance": 2
	},
	...
]
```

### Get All CODECs
```
GET /v1.0/codec

Success: 200/OK
[
	{
	    "GUID": "[guid]",
	    "VendorGUID": "[vendorguid]",
	    "Key": "[key]",
	    "Name": "[name]",
	    "Version": "[version]",
	    "Uri": "[uri]",
	    "CreatedUtc": "2023-04-26T19:16:28.465277",
	    "LastModifiedUtc": "2023-04-26T19:16:28.465277"
	},
	...
]
```

### Get CODEC by Key
```
GET /v1.0/codec/[key]

Success: 200/OK
{
    "GUID": "[guid]",
    "VendorGUID": "[vendorguid]",
    "Key": "[key]",
    "Name": "[name]",
    "Version": "[version]",
    "Uri": "[uri]",
    "CreatedUtc": "2023-04-26T19:16:28.465277",
    "LastModifiedUtc": "2023-04-26T19:16:28.465277"
}

Failure: 404
{
    "Message": "The requested object was not found."
}
```

### Find Closest CODEC Match
```
GET /v1.0/codec/match/[key]

Success: 200/OK
{    
	"GUID": "[guid]",
    "VendorGUID": "[vendorguid]",
    "Key": "[key]",
    "Name": "[name]",
    "Version": "[version]",
    "Uri": "[uri]",
    "CreatedUtc": "2023-04-26T19:16:28.465277",
    "LastModifiedUtc": "2023-04-26T19:16:28.465277"
    "EditDistance": 2
}
```

### Find Closest CODEC Matches
```
GET /v1.0/codec/match/[key]?results=10

Success: 200/OK
[
	{
		"GUID": "[guid]",
	    "VendorGUID": "[vendorguid]",
	    "Key": "[key]",
	    "Name": "[name]",
	    "Version": "[version]",
	    "Uri": "[uri]",
	    "CreatedUtc": "2023-04-26T19:16:28.465277",
	    "LastModifiedUtc": "2023-04-26T19:16:28.465277"
	    "EditDistance": 2
	},
	...
]
```


## Version History

Please refer to CHANGELOG.md for details.
