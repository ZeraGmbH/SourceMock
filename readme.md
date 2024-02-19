# WebSAM Device APIs

This Repo is meant to contain the APIs for everything that is intended to run on ZERA devices in the long run. This includes, but isn't limited to, Sources, Refernece Meters, Error Calulators. Currently, these are all part of one API, but different controllers.

The existing implementations wrap old interfaces to the devices mentiond above or simulate their behaiviour.

This repository is primarily serves as a submodule to other projects, namely [WebSAM](https://github.com/ZeraGmbH/websam) and [ZENUX](https://github.com/ZeraGmbH/meta-zera). For information on how to set this project up, refer to these projects.

# Configuration

Various web servers in this repository need a dedicated configuration in the overall `appSettings.json`.

- [Barcode Reader API](BarcodeApi/readme.md)
