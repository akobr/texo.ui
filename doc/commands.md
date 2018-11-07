# Command configuration

List of example commands and their configuration.

## Current directory

Gets and Sets current / working directory.

### current-directory | cd | cdir | chdir

* **[qD]** set
  * **[a]** \<path\>
  * **[q]** get

## Environment variables

Manager for global variables in Text.UI.

### environment | env | variable | var

* **[qD]** list
* **[q]** get
  * **[a]** \<name\>
* **[q]** set
  * **[a]** \<name\>
  * **[a]** \<value\>
* **[q]** remove|delete
  * **[a]** \<name\>

## Setting

Command for manage settings of the Texo.UI.

### setting | config

* **[qD]** list
* **[q]** {category}
  * **[q]** list
  * **[q]** {property}
    * **[a]** \<value\>

## File manager

Text base file manager with history and stash.

### file-manager | file | fmanager | fman | fm

* **[q]** list
  * **[a]** \<path\>
* **[q]** add
  * **[o*]** -i--ignore
    * **[a]** \<path\>
  * **[a+]** \<path\>>
* **[q]** remove|rm
  * **[a+]** \<path\>
* **[q]** makedir|mkdir|mdir|md
  * **[a+]** \<path\>
* **[q]** stash
  * **[q]** list
  * **[qD]** push
    * **[a?]** \<name\>
  * **[q]** pop
    * **[a?]** \<id|name\>
  * **[q]** apply
    * **[o?]** -o--override
    * **[o?]** -m--merge
    * **[a?]** \<id|name\>
  * **[q]** drop
    * **[a+]** \<id|name\>
  * **[q]** clear
* **[q]** history
  * **[qD]** list
  * **[q]** undo
  * **[q]** redo
* **[q]** apply
  * **[o?]** -p--preview
  * **[q]** copy
    * **[o?]** -d--keep-directories
    * **[o?]** -r--relative
      * **[a]** \<path\>
    * **[a]** \<path\>
  * **[q]** rename
    * **[o?] -e-extension
      * **[a]** \<extension\> 
    * **[q]** list
    * **[a]** \<name|format\>
  * **[q]** move
    * **[o?]** -d--keep-directories
    * **[o?]** -r--relative
      * **[a]** \<path\>
    * **[a]** \<path\>
  * **[q]** remove|delete
    * **[o?]** -p--permanent
  * **[q]** search
    * **[o?]** -e-expression
    * **[a+]** \<term\>
  * **[q]** replace
    * **[o?]** -e-expression
    * **[a]** \<from\>
    * **[a]** \<to\>
  * **[q]** pack|zip
    * **[a]** \<path\>   

## Search (Visual Studio)

Search in code, with knowlidge about business logic. Need support for real-time execution / intellisence.

### search

* **[o]** -f--file
* **[o]** -t-type
* **[o]** -m--member
* **[oD]** -s--symbol
* **[o]** -w--widget
* **[o]** -c--component
* **[o]** -a--action
* **[o]** -s--service
* **[a+]** \<term\>

## Duplicate references (Visual Studio)

Simple tool to find duplicate references in csproj files.

### duplicate-refecence | dupref | dref

* **[a+]** \<filePath\>

## Nuget package manager (Visual Studio)

Tool to manage nuget packages in solution / project. 

### nuget-manager | nmanager | nman | nm | nuget

* **[o?]** -p--path 
  * **[a]** \<path\>
* **[qD]** list
  * **[a+]** \<package|filter\>
* **[q]** update
  * **[o]** -v--version
    * **[a]** \<version\>
  * **[a+]** \<package|filter\>
* **[q]** remove|delete
  * **[a+]** \<package|filter\>
* **[q]** add
  * **[a+]** \<package\>

## Refactoring (Visual Studio)

Do a refactoring in Visual Studio.

### refactor

* **[q]** {refactorName}
  * **[a\*]** {argument}

## Create a item (Visual Studio)

Adds a new item to project in Visual Studio.

### create

* **[q]** {itemName}
  * **[a\*]** {argument}