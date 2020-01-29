# WinLess Core

WinLessCore is a fork of the popular WinLess application [http://winless.org](http://winless.org) that has been ported to use .NET Core 3.1

## Purpose & Intention
The scope of this fork was for me to use this tool as an opportunity to play around with the latest .NET Core 3 release using WinForms and see what a migration process would imply.
My future intentions are to fix some previusly reported issues in the original repository and keep the application and dependencies up to date whenever I have some spare time :)

## Using a globally installed less
To use a globally installed LESS, instead of the one bundled with WinLess, you should follow the following steps:

1. Install [Node.js](http://nodejs.org/), which comes bundled with NPM.
2. Open command prompt.
3. Execute `npm install less -g`
4. Execute `npm install less-plugin-clean-css -g`
5. Choose `use globally installed less` in WinLess settings.
