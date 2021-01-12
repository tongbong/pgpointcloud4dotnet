# pgpointcloud4dotnet

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=tongbong_pgpointcloud4dotnet&metric=alert_status)](https://sonarcloud.io/dashboard?id=tongbong_pgpointcloud4dotnet)

Schema
| Attribute | Supported |
|:---------------:|:-------------:|
| position        | ongoing       |
| size            | x             |
| name            | ongoing       |
| description     | x             |
| active          | ongoing       |
| interpretation  | ongoing       |
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
| ongoing         | x             |

Patch
| Compression  | Deserialization | Serialization |
|:------------:|:---------------:|:-------------:|
| Uncompressed | ongoing         | x             |
| Dimensional  | ongoing         | x             |
| LAZ          | x               | x             |
