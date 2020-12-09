![Nuget](https://img.shields.io/nuget/v/TruePeople.SharePreview)

# TruePeople.SharePreview
Share preview URLs with non-Umbraco users! It supports multi-cultural and single-culture sites.

## Installation
Install the package via [NuGet](https://www.nuget.org/packages/TruePeople.SharePreview) or download it from [Our Umbraco](https://our.umbraco.com/packages/backoffice-extensions/truepeoplesharepreview/).


## Settings
In the settings section you will see a new tree node 'Shareable Preview settings', here you can manage the encryption key that will be used to generate the shareable preview URL.
You can also manage the URL a user will get redirected to when the link isn't valid anymore.

## How to share a preview?
After installation you will see a new button next to the 'Preview' button on your content nodes.
**When the newest version is published, this button will be disabled.**
Clicking on this button in a non multi lingual environment will directly copy the link to your clipboard,
but when you open this in a multi lingual environment it will open a popup where you can see all variants with a non-published version.

## How long will the link be valid for?
The link generated will remain valid until the version that the link was generated on has been published.
When someone tries to access the link then, they will be redirected to the URL you configured in the settings.

## How does it work?
The generated link consists of the node id, version id, culture and the date the version has been saved on.
We render the correct content by using a custom route and having our own UmbracoVirtualNodeRouteHandler implemented.
The handler takes care of setting the right VariationContext in previews ,checking if the link is still valid and for setting the preview content to the current request.

## How to run this on your local machine?
Clone this repository to your machine.
Run the build.ps1 script that is located in the package folder.
Specify what version you would like to give the build, and if you want to package it up for Umbraco or NuGet.
This will generate the packages inside the same folder.


** We are working on implementing segment support in this package.**

# Changelog

## v1.0.2
- #4 Change target framework to 4.7.2
- #5 Remove the previewbadge from the output html

## v1.0.1

- #2 Make sure the request is always in preview mode

## v1.0.0

- Initial package release
	
---
