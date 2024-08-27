# NauraaBot

## Introduction

A card fetching Discord bot for Altered TCG

Add it to your Discord server by clicking [this link](https://discord.com/oauth2/authorize?client_id=1214980379216318606&permissions=824633838592&scope=bot), or look below if you're looking to self host

## Usage

### Basic usage
In any channel on a server where the bot is present, summon it by typing

```
{{Card Name}}
```

You can also get the rare in faction version by typing :

```
{{Card Name|R}}
``` 

And the out of faction one with :

```
{{Card Name|OOF}}
OR
{{Card Name|R, OOF}}
```

You can also specify the language in which to get the results in the case you have a card that has the same name in multiple languages (ex : Kitsune, Parvati) like this
```
{{Card Name||FR}}
OR
{{Card Name|R|FR}}
```

Prefixing `!` before the card name will get you the full card image in the reply instead of the details in text form.

Prefixing `@` before the card name will show you the card name in all supported languages.

### Random card fetching

You can get a random card by replacing the card's name in your query by `rand()`. All search parameters work with this. For instance, `{{rand()|C,MU|FR}}` will get a random French Muna common.

### Specific unique fetching

There are two ways to fetch a specific unique :

#### Unique ID Searching

You can search for a specific unique by specifying its ID like this : `{{ALT_COREKS_B_YZ_11_U_4053}}`. When searching like this, the only parameter you may specify is the language, like this `{{ALT_COREKS_B_YZ_11_U_4053|FR}}`. Attempting to specify a rarity or a faction will cause an error. You can still use the `!` or `@` prefixes however.

Please note that while the bot has access to most uniques at this time, it only updates new uniques each morning at 1AM CET, and some uniques may not be available at all due to Altered's API limitations. Furthermore, once the uniques are made private, the existing uniques might be removed from the bot if there is such a request from Equinox or a significant part of the community.

#### Search by name and set ID/number

You can also search a specific unique by specifying the name of the card (partially or fully), then the set and number of the unique as the first set of options. 

For instance, `{{Daughter of Yggdrasil|COREKS, 5111}}` will get the 5111th Daughter of Yggdrasil unique from the Kickstarter set (which is equivalent to `{{ALT_COREKS_B_MU_12_U_5111}}`)

## Requirements

- .NET 6 (SDK and runtime)
- .NET EF Version 6.0.27 (`dotnet tool install --global dotnet-ef --version 6.0.27 `)

## Compiling

- Go to the `src` folder
- Copy the `config.yml.dist` r and rename the copy to `config.yml`
- Modify the `config.yml` according to your needs. Make sure you fill in your bot token.
- Run `dotnet ef database update` or `dotnet-ef database update`
- Run `dotnet publish -c Release -p:PublishSingleFile=true -r <RID>` where \<RID\> is your platform's [runtime ID](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog). The application will be compiled in : `\bin\Release\net6.0\<RID>\publish`

## FAQ

### Why not use an Axiom robot companion name ?

Muna > Axiom

### Will there be a Docker image available ?

Possibly, but this is not a priority right now.

### What about a pre-compiled version ?

This is more likely to come soon, but I need to figure out how to handle database migrations on a compiled executable. I know about `db.Migrate()`, but I've seen many different disclaimers to not use that ever in production. Bundling an empty migrated database with the releases would also be an option, but that doesn't sound optimal either. In short, I need to find the cleanest way to handle this.

# Acknowledgments

Thanks to `mangofeet` from the Altered Discord for answering my questions about the API.

Altered is the property of Equinox. This project is an unofficial fanmade endeavor which is no way affiliated to or endorsed by Equinox.
