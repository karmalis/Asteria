# Asteria

## Overview

A basic application for reading file contents, splitting words based on a pre-set character limit and writing the results to another file.

## Usage

Application can be run through the command line.

For basic usage see

`Asteria.exe /help`

### Example usage

Suppose there is a file called _input.txt_ and we want the results be written into _output.txt_ with a 13 character limit. The actual command for running
the application would be:

`Asteria.exe /in:"input.txt" /out:"output.txt" /cnum:13`

The command line argument parser supports multiple ways of calling up the application, so the following is also valid:

`Asteria.exe -in="input.txt" -out="output.txt" -cnum=13`
