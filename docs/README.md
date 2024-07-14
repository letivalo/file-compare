
# File Compare Project

A simple file comparison utility that compares files between two source directories and optionally copies or moves the smaller files to a destination directory.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Requirements](#requirements)
- [Setup](#setup)
- [Usage](#usage)
- [Example](#example)
- [License](#license)
- [Contact](#contact)

## Overview

The File Compare Project is a console application that allows users to compare identical files between two source directories, and move the smaller of the files between the two to a new directory. I got the idea for this project after re-encoding a number of video files to HVEC as a batch using handbreak, but noticing that sometimes the original video files would be smaller than those which I've re-encoded to the more space effiecient codec. This project originally started out as a simple PowerShell script but I've since expanded it out a bit and re-wrote it in C# for fun. This program should be pretty stable, but it's still a WIP, feel free to reach out if you have any sugestions or issues.

## Features

- Load configuration from a JSON file.
- Validate source and destination directories.
- Compare files between two source directories.
- Copy or move smaller files to a destination directory.
- Optionally delete original source directories after moving files.
- Log the operations performed.

## Requirements

- .NET 8.0 SDK or later
- Newtonsoft.Json library

## Setup

1. Clone the repository:
    \`\`\`sh
    git clone https://github.com/letivalo/file-compare-project.git
    \`\`\`

2. Navigate to the project directory:
    \`\`\`sh
    cd file-compare-project
    \`\`\`

3. Restore the necessary packages:
    \`\`\`sh
    dotnet restore
    \`\`\`

## Usage

1. Update the \`config.json\` file with your directories:
    \`\`\`json
    {
      "SourceFolder1": "path/to/source1",
      "SourceFolder2": "path/to/source2",
      "DestinationFolder": "path/to/destination",
      "LogFile": "path/to/logfile.log"
    }
    \`\`\`

2. Build the project:
    \`\`\`sh
    dotnet build
    \`\`\`

3. Run the project:
    \`\`\`sh
    dotnet run --project src/FileCompareProject
    \`\`\`

## Example

\`\`\`
dotnet run --project src/FileCompareProject
\`\`\`

Sample output:

\`\`\`
File Comparison v0.1.0 by letivalo
-------------------------------------------------------
Config file path: config.json
Source Folder 1: path/to/source1 - Size: 1000 MB
Source Folder 2: path/to/source2 - Size: 700 MB
Destination Folder: path/to/destination
Estimated final size of destination folder: 500 MB
-------------------------------------------------------
Do you want to proceed with the file operations? (Y/N) y
-------------------------------------------------------
Do you want to (1) Copy the smaller files or (2) Move the smaller files? 1
Beginning file operations, this may take a moment...
File operations completed.
\`\`\`

## License

This project is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License. See the [LICENSE](LICENSE) file for details.

## Contact

For any inquiries, please feel free to shoot me an email at [letivalo.dev@tuta.io](mailto:letivalo.dev@tuta.io) or visit the project repository at
[project repository](https://github.com/letivalo/filecompare/issues).

---

Created by letivalo.
