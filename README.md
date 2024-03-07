# NauraaBot

## Introduction

A card fetching Discord bot for Altered TCG

Add it to your Discord server by cicking [this link](http://placeholder), or look below if you're looking to self host

## Usage

In any channel on a server where the bot is present, summon it by typing

```
{{Card Name}}
```

This should be the **exact English** name of the card you're looking for. Support for other languages and approximate search terms is planned for a future version.

You can also get the rare in faction version by typing :

```
{{Card Name|R}}
``` 

And the out of faction one with :

```
{{Card Name|R, AX}}
```

Where you replace `AX` with the first two letters of the out of faction faction.

## Requirements

- .NET 6 (SDK and runtime)
- .NET EF Version 6.0.27 (`dotnet tool install --global dotnet-ef --version 6.0.27 `)

## Planned features

- Support for other languages than English (top priority)
- Support for approximate search terms
- Message throttling per user
- Support for promo cards fetching
- And more

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
