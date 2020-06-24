# phosphoRS-cli
A command line interface of phosphoRS (version 3.1).


## Denpendency
Download the phosphoRS library (`IMP.PhosphoRS.dll`) from Research Institute of Molecular Pathology (IMP):
https://ms.imp.ac.at/index.php?action=phosphors.

Place the DLL file in the `src/lib` directory (source code), and in the same directory as the executable file (released binary).

## Usage
```
phosphoRS:
  PhosphoRS Commandline Tool

Usage:
  phosphoRS [options]

Options:
  -i, --input <INPUT>                  Input file (.XML). This option will be used first.
  -o, --output <OUTPUT>                Output path (.XML, .JSON or .CSV).
  -p, --peptide <PEPTIDE>              Peptide identification result file (.PepXML). This option will be ignored if --input is given.
  -s, --spectra <SPECTRA>              MS2 Spectra file (.MGF). This option will be ignored if --input is given.
  -a, --activation <ACTIVATION>        Activation type (CID, HCD, ECD or ETD). Default CID. This option will be ignored if --input is given.
  -t, --tolerance <TOLERANCE>          Mass tolerance. Default 0.02. This option will be ignored if --input is given.
  -I, --max-isoform <MAX-ISOFORM>      Maximun number of isoforms. Default 200. This option will be ignored if --input is given.
  -P, --max-ptm <MAX-PTM>              Maximun number of PTMs. Default 20. This option will be ignored if --input is given.
  -n, --neutral-loss <NEUTRAL-LOSS>    Score neutral loss. Default on. This option will be ignored if --input is given.
  --version                            Display version information
```

Example input and output files are provided in the `data` directory.

## Accessories
R scripts are provided in the `scripts` directory to generate an input XML file from a PSM list file and MGF files.


## License
phosphoRS-cli is distributed under a MIT license. See the LICENSE file for details.

The phosphoRS library (`IMP.PhosphoRS.dll`) is provided as freeware by its copyright owner, Research Institute of Molecular Pathology (IMP), under their license.
