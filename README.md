# pgpointcloud4dotnet

Schema
| Attribute | Supported |
|:---------------:|:-------------:|
| position        | x             |
| size            | x             |
| name            | x             |
| description     | x             |
| active          | x             |
| interpretation  | x             |
| minimum         | x             |
| maximum         | x             |
| offset          | x             |
| scale           | x             |
| endianness      | x             |
| uuid            | x             |
| parent_uuid     | x             |


Point
| Deserialization | Serialization |
|:---------------:|:-------------:|
| x               | x             |
| x               | x             |
| x               | x             |

Patch
| Compression  | Deserialization | Serialization |
|:------------:|:---------------:|:-------------:|
| Uncompressed | x               | x             |
| Dimensional  | x               | x             |
| LAZ          | x               | x             |
