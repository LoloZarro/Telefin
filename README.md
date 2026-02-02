<h1 align="center">Telefin Plugin for Jellyfin</h1>
<h3 align="center">Enable Telegram Notification for Jellyfin events!</h3>

---

<p align="center">
  <img width="1536" height="1024" alt="GithubBannerTelefin" src="https://github.com/user-attachments/assets/6238e069-c406-4bee-8175-75fd1708e314" />
</p>
<br/>
<p align="center">
  <a href="https://github.com/LoloZarro/Telefin/actions">
    <img alt="GitHub Workflow Status" src="https://img.shields.io/github/actions/workflow/status/LoloZarro/Telefin/build_telefin.yml?branch=main&logo=github">
  </a>
  <a href="https://github.com/LoloZarro/Telefin/releases">
    <img alt="Current Release" src="https://img.shields.io/github/release/LoloZarro/Telefin.svg"/>
  </a>
  <a href="https://github.com/LoloZarro/Telefin">
    <img alt="gitHub Stars" src="https://img.shields.io/github/stars/LoloZarro/Telefin?style=flat">
  </a>
  <a href="https://github.com/LoloZarro/Telefin">
    <img alt="Downloads" src="https://img.shields.io/github/downloads/LoloZarro/Telefin/total">
  </a>
  <a href="https://github.com/LoloZarro/Telefin/search?l=c%23">
    <img alt="GitHub top language" src="https://img.shields.io/github/languages/top/LoloZarro/Telefin?color=%23239120&label=.NET&logo=csharp">
  </a>
  <a href="https://github.com/LoloZarro/Telefin">
    <img alt="GPLv3 License" src="https://img.shields.io/github/license/LoloZarro/Telefin.svg"/>
  </a>
</p>

---

## About

**Telefin** is a Jellyfin plugin that enables **Telegram notifications for Jellyfin events**.
It allows Jellyfin administrators to configure **Telegram bots on a per-user basis**, giving each user full control over which events they want to be notified about and how those notifications look.

Every supported Jellyfin event can be individually enabled or disabled. Each event message is fully customizable using **placeholders**. This enables both simple alerts and highly detailed, personalized notifications.

Telefin is designed to be lightweight, flexible, and easy to configure directly from the Jellyfin admin interface.

## Features

* Telegram notifications for Jellyfin server events (see: [supported events](https://github.com/LoloZarro/Telefin/wiki#supported-jellyfin-events))
* Configuration through the Jellyfin plugin settings interface
* Per-user configuration
  * Separate bot configuration for each user
* Event-level control
  * Enable or disable notifications for each event
* Event-specific messages
  * Each event has it's own message
* Event-specific variables
  * Each event has it's own placeholder variables that can be used in the message (see: [variables](https://github.com/LoloZarro/Telefin/wiki/Message-Variables))

## Screenshots
<div style="display:flex; gap:15px; align-items:flex-start;">
  <img
    alt="Configuration"
    src="https://github.com/user-attachments/assets/420262d4-918d-4bb4-a94c-16b1b7413a8d"
    style="height:600px; width:auto;"/>
  <img
    alt="Series"
    src="https://github.com/user-attachments/assets/9a0778c6-80e2-4848-96c0-301d0dff4040"
    style="height:600px; width:auto;"/>
  <img
    alt="Movie"
    src="https://github.com/user-attachments/assets/12526ed9-5c2b-4d42-8fa8-d38ab0e4ea06"
    style="height:600px; width:auto;"/>
</div>

## Installation

The installation guide can be found [here](https://github.com/LoloZarro/Telefin/wiki/Installation).

## Documentation

Documentation can be found in the official [wiki](https://github.com/LoloZarro/Telefin/wiki).

## Support & Feedback

If you encounter issues, have feature requests, or want to discuss ideas:
* Use [Discussions](https://github.com/LoloZarro/Telefin/discussions) for questions and suggestions
* Open an [Issue](https://github.com/LoloZarro/Telefin/issues) for bugs or technical problems

## Licence

This plugin is released under the [GPL-3.0](https://github.com/LoloZarro/Telefin/blob/main/LICENSE) License.

## Star History

[![Star History Chart](https://api.star-history.com/svg?repos=LoloZarro/Telefin&type=date&legend=bottom-right)](https://www.star-history.com/#LoloZarro/Telefin&type=date&legend=bottom-right)
