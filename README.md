# Synology DDNS Updater

[![CI Build][ci-build-badge]][ci-build]
[![codecov][codecov-badge]][codecov]
[![Docker Image Version (tag latest semver)][docker-image-badge]][docker-image]

This repository contains an ASP.NET Core application used to adapt [Namecheap][namecheap] DDNS update responses to
something that a Synology [Customized DDNS Provider][synology-ddns] expects. This is done by calling the
[Namecheap API][namecheap-ddns-api] directly on your behalf with the values you provide and then adapting the response.

The implementation of this project was heavily inspired by [this][encodebudenet-post] blog post.

## Why?

While the solutions from the various blog posts (see [here][brettterpstra-post] and [here][encodebudenet-post]) work
great, updates to Synology DSM often wipe out some of the configurations breaking the DDNS configuration. This can
leave your domain pointing to an incorrect IP address and certificates failing to renew due to resolution failures. By
hosting a dedicated DDNS update adapter, we can use a [Customized DDNS Provider][synology-ddns] out of the box and not
worry about custom configuration getting cleared by Synology DSM updates.

## How?

The first things you need to do are to [enable DDNS on your domain][namecheap-enable-ddns] and to
[setup a host record][namecheap-setup-host].

An instance of this application is hosted at <https://synologyddnsupdater.azurewebsites.net>, but you can easily build
and deploy the application yourself if you so choose. If you deploy it yourself, you would need to adjust the host value
in the following instructions.

Follow the instructions for [Setting up a Customized DDNS Provider][synology-ddns] to setup a new Customized DDNS
Provider using the following values:

* Service Provider: `Namecheap-<domain identifier>` (Whatever you want really. It's just a name.)
* Query URL: `https://synologyddnsupdater.azurewebsites.net/namecheap/ddns/update?host=__USERNAME__&domain=__HOSTNAME__&password=__PASSWORD__&ip=__MYIP__`

When configuring your DDNS, use something like the following:

| DDNS form value         | Value                           | Example          |
|-------------------------|---------------------------------|------------------|
| Hostname                | Namecheap domain name           | `yourdomain.tld` |
| Username/Email          | Namecheap host hame             | `@`              |
| Password/Key            | Namecheap DDNS password         |                  |
| External Address (IPV4) | Auto (your external IP address) |                  |

Note: I would have preferred to use the Namecheap domain name for the "Username/Email", but the DDNS form doesn't allow
the use of the `@` symbol in the "Hostname" field.

## Hosting the application yourself

You can follow the instructions [here][synology-ddns-update-service] to learn how to configure and run the service
yourself.

[brettterpstra-post]: https://brettterpstra.com/2021/08/26/custom-urls-for-your-synology-with-namecheap/ "Custom URLs for your Synology with Namecheap"
[ci-build]: https://github.com/craigktreasure/SynologyDdnsUpdater/actions/workflows/CI.yml
[ci-build-badge]: https://github.com/craigktreasure/SynologyDdnsUpdater/actions/workflows/CI.yml/badge.svg?branch=main
[codecov]: https://codecov.io/gh/craigktreasure/SynologyDdnsUpdater
[codecov-badge]: https://codecov.io/gh/craigktreasure/SynologyDdnsUpdater/branch/main/graph/badge.svg?token=28F4PZLPN8
[docker-image]: https://hub.docker.com/r/craigktreasure/synologyddnsupdater "craigktreasure/synologyddnsupdater"
[docker-image-badge]: https://img.shields.io/docker/v/craigktreasure/synologyddnsupdater/latest?label=craigktreasure%2Fsynologyddnsupdater
[encodebudenet-post]: https://en.code-bude.net/2022/02/17/set-up-namecheap-com-ddns-in-synology-dsm/ "Set up Namecheap.com DDNS in Synology DSM"
[namecheap]: https://namecheap.com "Namecheap"
[namecheap-ddns-api]: https://www.namecheap.com/support/knowledgebase/article.aspx/29/11/how-to-dynamically-update-the-hosts-ip-with-an-http-request/ "How to dynamically update the host's IP with an HTTP request?"
[namecheap-enable-ddns]: https://www.namecheap.com/support/knowledgebase/article.aspx/595/11/how-do-i-enable-dynamic-dns-for-a-domain/ "How do I enable Dynamic DNS for a domain?"
[namecheap-setup-host]: https://www.namecheap.com/support/knowledgebase/article.aspx/43/11/how-do-i-set-up-a-host-for-dynamic-dns/ "How do I set up a Host for Dynamic DNS?"
[synology-ddns-update-service]: /src/Synology.Ddns.Update.Service/README.md
[synology-ddns]: https://kb.synology.com/en-br/DSM/help/DSM/AdminCenter/connection_ddns "Synology DDNS"
