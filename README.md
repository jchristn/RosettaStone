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

```

### Get CODEC by Key
```
GET /v1.0/codec/[key]

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
  "LastModifiedUtc": "2023-04-26T19:16:28.465277",
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
    "LastModifiedUtc": "2023-04-26T19:16:28.465277",
    "EditDistance": 2
  },
  ...
]
```

### Find Closest Vendor and CODEC by Full Sector Zero Payload
```
GET /v1.0/full/match/[key]

Success: 200/OK
{
    "Key": "ACTGACTGACTGACTGACTGACTGACTGAAACTAGCTAGCTAGCTAGCTAGCTAGCTAGCCC",
    "Left": "ACTGACTGACTGACTGACTGACTGACTGAAACTAG",
    "Right": "GAAACTAGCTAGCTAGCTAGCTAGCTAGCTAGCCC",
    "Vendor": {
        "GUID": "74f7cadf-1496-4441-9d52-3417ab2619dd",
        "Key": "ACTGACTGACTGACTGACTGACTGACTGAC",
        "Name": "Vendor 1",
        "ContactInformation": "100 S Main St, San Jose, CA 95128",
        "CreatedUtc": "2023-04-26T19:16:28.382196",
        "LastModifiedUtc": "2023-04-26T19:16:28.382196",
        "EditDistance": 5
    },
    "Codec": {
        "GUID": "4207c038-ea01-4ade-bb80-cea2aa9e0058",
        "VendorGUID": "5c0847de-b4be-4b4c-bcb9-adc899b27c11",
        "Key": "TAGCTAGCTAGCTAGCTAGCTAGCTAGCAC",
        "Name": "My CODEC",
        "Version": "v3.0.0",
        "Uri": "https://codec1.com",
        "CreatedUtc": "2023-04-26T19:16:28.465358",
        "LastModifiedUtc": "2023-04-26T19:16:28.465358",
        "EditDistance": 6
    }
}
```

### Find Closest Vendors and CODECs by Full Sector Zero Payload
```
GET /v1.0/full/matches/[key]?results=10

Success: 200/OK
{
    "Key": "ACTGACTGACTGACTGACTGACTGACTGAAACTAGCTAGCTAGCTAGCTAGCTAGCTAGCCC",
    "Left": "ACTGACTGACTGACTGACTGACTGACTGAAACTAG",
    "Right": "GAAACTAGCTAGCTAGCTAGCTAGCTAGCTAGCCC",
    "Vendors": [
        {
            "GUID": "57ED37C2-2AA5-402E-BEAF-3A7E0E507614",
            "Key": "ACTGACTGACTGACTGACTGACTGACTGAC",
            "Name": "Vendor 1",
            "ContactInformation": "100 S Main St, San Jose, CA 95128",
            "CreatedUtc": "2023-05-30T22:34:46",
            "LastModifiedUtc": "2023-05-30T22:34:46",
            "EditDistance": 5
        },
        ...
    ],
    "Codecs": [
        {
            "GUID": "2CB643C4-F122-488B-A0C5-B0B851AD1542",
            "VendorGUID": "30F88C72-4542-47E9-8BD4-12EDA54144CA",
            "Key": "TAGCTAGCTAGCTAGCTAGCTAGCTAGCAC",
            "Name": "My CODEC",
            "Version": "v3.0.0",
            "Uri": "https://codec1.com",
            "CreatedUtc": "2023-05-30T22:34:46",
            "LastModifiedUtc": "2023-05-30T22:34:46",
            "EditDistance": 6
        },
        ...
    ]
}
```

## Version History

Please refer to CHANGELOG.md for details.
