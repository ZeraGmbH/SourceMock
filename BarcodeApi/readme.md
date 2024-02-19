# Barcode Reader API

Barcode readers will be connected to the system as a keyboard or generally as a human interface device (HID). To use a barcode reader a path to the related device must be configured. It is recommended to use the symbolic link file which is more or less stable unique name to the device.

```json
  "BarcodeReader": {
    "Device": "/dev/input/by-id/usb-WSM_Keyboard_00000000011C-event-kbd"
  }
```

Depending on the linux operating system used it may be neccessary to connect to the input device directly since the symbolic link is not always created. In principle this path may change on each connect.

```json
  "BarcodeReader": {
    "Device": "/dev/input/event11"
  }
```

The barcode reader implementation uses raw HID events to interpret incoming data and not keystrokes.
