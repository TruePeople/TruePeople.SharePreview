# TruePeople.SharePreview
Share previews with people that don't have a login to your Umbraco environment! It supports multi-cultural and single-culture sites.

## Installation
Install the package via Nuget or download it from Our.Umbraco.

## Settings
In the settings section you will see a new tree node 'Shareable Preview settings', here you can manage the encryption key that will be used to generate the shareable preview url.
You can also manage the Url a user will get redirected to when the link isn't valid anymore.

## How to share a preview?
After installation you will see a new button next to the 'Preview' button on your content nodes.
**When the newest version is published, this button will be disabled.**
Clicking on this button in a non multi lingual environment will directly copy the link to your clipboard,
but when you open this in a multi lingual environment it will open a popup where you can see all variants with a non published version.

## How long will the link be valid for?
The link generated will remain valid until the version that the link was generated on has been published.
When someone tries to acces the link then, they will be redirected to the url you configured in the settings.

## How does it work?
The generated link consists of the nodeid, versionid, culture and the date the version has been saved on.
We render the correct content by using a custom route and having our own UmbracoVirtualNodeRouteHandler implemented.
The handler takes care of setting the right VariationContext in previews ,checking if the link is still valid and for setting the preview content to the current request.

## How to run this on your local machine?
Clone this repository to your machine.
Run the build.ps1 script that is located in the package folder.
When you run the script you can specify what version you would like to give it and if you want to package it up for umbraco or nuget.
This will then generate the packages inside the same folder.