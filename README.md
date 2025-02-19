# CustomDewCollector

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

CustomDewCollector is a mod for **7 Days to Die** that empowers server admins with the ability to customize dew collector slot sizes and increase the rate at which water is collected. This makes dew collectors a more viable option in the early stages of the game, providing players with easier access to water resources.

## Compatability
Tested on v1.0

## Features

- **Configurable Slot Sizes**: Adjust the slot sizes of dew collectors to suit your server's gameplay style and balance needs.
- **Enhanced Collection Rate**: Increase the rate at which water is collected, making dew collectors a practical choice for early-game hydration.

## Installation

1. **Download the Mod**:
   - Go to the [Releases](https://github.com/VirtualVerse-LLC/CustomDewCollectorSize/releases/) section and download the latest ZIP file.

2. **Install the Mod**:
   - Extract the contents of the ZIP file and place the `CustomDewCollector` folder into your `Mods` directory located in the game’s root directory.


## Configuration

The `config.xml` file allows you to customize the following settings:

- `Columns`: Defines the number of slot columns available in the dew collector.
- `Rows`: Defines the number of slot rows available in the dew collector.

Example:

```xml
<config>
    <columns>3</columns>
    <rows>2</rows>
</config>
```

The `Config/XUi/windows.xml` needs to match your `config.xml` file :

Example
```xml
<config>
	<set xpath="//window[@name='windowDewCollector']/rect/grid/@cols">3</set>
	<set xpath="//window[@name='windowDewCollector']/rect/grid/@rows">2</set>
</config>
```

The `Config/blocks.xml` can be used to modify the collection rate :

Example
```xml
<config>
    <set xpath="/blocks/block[@name='cntDewCollector']/property[@name='MinConvertTime']/@value">1800</set>
    <set xpath="/blocks/block[@name='cntDewCollector']/property[@name='MaxConvertTime']/@value">3600</set>
</config>
```
