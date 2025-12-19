export default function (view) {

    const TelefinConfig = {
        pluginUniqueId: '91ce115a-3f38-49bc-b36c-0f688b1495c7',

        notificationType: {
            values: {
                "ItemAdded": ["Item Added", "Movies", "Series", "Seasons", "Episodes", "Albums", "Songs", "Books"],
                "ItemDeleted": ["Item Deleted", "Movies", "Series", "Seasons", "Episodes", "Albums", "Songs", "Books"],
                "PlaybackStart": ["Playback Start", "Movies", "Episodes"],
                "PlaybackProgress": ["Playback Progress (recommended: disabled)", "Movies", "Episodes"],
                "PlaybackStop": ["Playback Stop", "Movies", "Episodes"],
                "SubtitleDownloadFailure": "Subtitle Download Failure",
                "AuthenticationFailure": "Authentication Failure",
                "AuthenticationSuccess": "Authentication Success",
                "SessionStart": "Session Start",
                "PendingRestart": "Pending Restart",
                "TaskCompleted": "Task Completed",
                "PluginInstallationCancelled": "Plugin Installation Cancelled",
                "PluginInstallationFailed": "Plugin Installation Failed",
                "PluginInstalled": "Plugin Installed",
                "PluginInstalling": "Plugin Installing",
                "PluginUninstalled": "Plugin Uninstalled",
                "PluginUpdated": "Plugin Updated",
                "UserCreated": "User Created",
                "UserDeleted": "User Deleted",
                "UserLockedOut": "User Locked Out",
                "UserPasswordChanged": "User Password Changed",
                "UserUpdated": "User Updated",
                "UserDataSaved": "User Data Saved"
            },

            defaultMessages: {
                "ItemAddedMovies": "🎬 {item.Name} ({item.ProductionYear})\n      added to library\n\n📽 {item.Overview}",
                "ItemAddedSeries": "📺 [Serie] {serie.Name} ({item.ProductionYear}) added to library\n\n📽 {item.Overview}",
                "ItemAddedSeasons": "📺 {season.Series.Name} ({item.ProductionYear})\n      Season {seasonNumber} added to library\n\n📽 {item.Overview}",
                "ItemAddedEpisodes": "📺 {episode.Series.Name} ({item.ProductionYear})\n      S{eSeasonNumber} - E{episodeNumber}\n      '{item.Name}' added to library\n\n📽 {item.Overview}",
                "ItemAddedAlbums": "🎵 [Album] {album.Name} ({item.ProductionYear}) added to library",
                "ItemAddedSongs": "🎵 [Audio] {audio.Name} ({item.ProductionYear}) added to library",
                "ItemAddedBooks": "📖 [Book] {item.name} added to library\n\n🖋️ {item.Overview}",
                "ItemDeletedMovies": "🗑️🎬 {item.Name} ({item.ProductionYear})\n      removed from library\n\n📽 {item.Overview}",
                "ItemDeletedSeries": "🗑️📺 [Serie] {serie.Name} ({item.ProductionYear}) removed from library\n\n📽 {item.Overview}",
                "ItemDeletedSeasons": "🗑️📺 {season.Series.Name} ({item.ProductionYear})\n      Season {seasonNumber} removed from library\n\n📽 {item.Overview}",
                "ItemDeletedEpisodes": "🗑️📺 {episode.Series.Name} ({item.ProductionYear})\n      S{eSeasonNumber} - E{episodeNumber}\n      '{item.Name}' removed from library\n\n📽 {item.Overview}",
                "ItemDeletedAlbums": "🗑️🎵 [Album] {album.Name} ({item.ProductionYear}) removed from library",
                "ItemDeletedSongs": "🗑️🎵 [Audio] {audio.Name} ({item.ProductionYear}) removed from library",
                "ItemDeletedBooks": "🗑️📖 [Book] {item.name} removed from library\n\n🖋️ {item.Overview}",
                "AuthenticationFailure": "🔒 Authentication failure on {eventArgs.Argument.DeviceName} for user {eventArgs.Argument.Username}",
                "AuthenticationSuccess": "🔓 Authentication success for user {eventArgs.Argument.User.Name} on {eventArgs.Argument.SessionInfo.DeviceName}",
                "PendingRestart": "🔄 Jellyfin is pending a restart.",
                "PlaybackProgressMovies": "👤 {eventArgs.Users[0].Username} is still watching on {eventArgs.DeviceName}:\n🎬 {eventArgs.Item.Name} ({eventArgs.Item.ProductionYear})",
                "PlaybackProgressEpisodes": "👤 {eventArgs.Users[0].Username} is still watching on {eventArgs.DeviceName}:\n🎬 {eventArgs.Item.Series.Name} ({eventArgs.Item.ProductionYear})\n      S{playbackSeasonNumber} - E{playbackEpisodeNumber}\n      '{eventArgs.Item.Name}'",
                "PlaybackStartMovies": "👤 {eventArgs.Users[0].Username} is watching on {eventArgs.DeviceName} ({eventArgs.Session.PlayState.PlayMethod}):\n🎬 {eventArgs.Item.Name} ({eventArgs.Item.ProductionYear})\n📺 [{eventArgs.Item.MediaType}] {eventArgs.Item.Genres}\n🕒 {duration}\n📽 {eventArgs.Item.Overview}",
                "PlaybackStartEpisodes": "👤 {eventArgs.Users[0].Username} is watching on {eventArgs.DeviceName} ({eventArgs.Session.PlayState.PlayMethod}):\n🎬 {eventArgs.Item.Series.Name} ({eventArgs.Item.ProductionYear})\n      S{playbackSeasonNumber} - E{playbackEpisodeNumber}\n      '{eventArgs.Item.Name}'\n📺 [{eventArgs.Item.MediaType}] {eventArgs.Item.Series.Genres}\n🕒 {duration}\n📽 {eventArgs.Item.Overview}",
                "PlaybackStopMovies": "👤 {eventArgs.Users[0].Username} stopped watching:\n🎬 {eventArgs.Item.Name} ({eventArgs.Item.ProductionYear})",
                "PlaybackStopEpisodes": "👤 {eventArgs.Users[0].Username} stopped watching:\n🎬 {eventArgs.Item.Series.Name} ({eventArgs.Item.ProductionYear})\n      S{playbackSeasonNumber} - E{playbackEpisodeNumber}\n      '{eventArgs.Item.Name}'",
                "PluginInstallationCancelled": "🔴 {eventArgs.Argument.Name} plugin installation cancelled (version {eventArgs.Argument.Version}):",
                "PluginInstallationFailed": "🔴 {eventArgs.InstallationInfo} plugin installation failed (version {eventArgs.VersionInfo}):\n{eventArgs.Exception}",
                "PluginInstalled": "🚧 {eventArgs.Argument.Name} plugin installed (version {eventArgs.Argument.Version})\n\nYou may need to restart your server.",
                "PluginInstalling": "🚧 {eventArgs.Argument.Name} plugin is installing (version {eventArgs.Argument.Version})",
                "PluginUninstalled": "🚧 {eventArgs.Argument.Name} plugin uninstalled",
                "PluginUpdated": "🚧 {eventArgs.Argument.Name} plugin updated to version {eventArgs.Argument.Version}:🗒️ {eventArgs.Argument.Changelog}\n\nYou may need to restart Jellyfin to apply the changes.",
                "SessionStart": "👤 {eventArgs.Argument.UserName} has started a session on:\n💻 {eventArgs.Argument.Client} ({eventArgs.Argument.DeviceName})\n",
                "SubtitleDownloadFailure": "🚫 Subtitle download failed for {eventArgs.Item.Name}",
                "TaskCompleted": "🧰 Task {eventArgs.Task.Name} completed: {eventArgs.Task.CurrentProgress}%\n🗒️ ({eventArgs.Task.Category}) {eventArgs.Task.Description}",
                "UserCreated": "👤 User {eventArgs.Argument.Username} created.",
                "UserDeleted": "🗑️ User {eventArgs.Argument.Username} deleted.",
                "UserLockedOut": "👤🔒 User {eventArgs.Argument.Username} locked out",
                "UserPasswordChanged": "👤 User {eventArgs.Argument.Username} changed his password.",
                "UserUpdated": "👤 User {eventArgs.Argument.Username} has been updated",
                "UserDataSaved": "👤 User {eventArgs.Argument.Username} data saved."
            },

            loadNotificationTypes: function (userConfig) {
                const temp = document.querySelector("#template-notification-type");
                const temp_without_textarea = document.querySelector("#template-notification-type-without-textarea");
                const subtemp = document.querySelector("#template-notification-subtype");
                const container = document.querySelector("[data-name=notificationTypeContainer]");
                container.innerHTML = '';

                const notificationTypeKeys = Object.keys(TelefinConfig.notificationType.values).sort();
                for (const key of notificationTypeKeys) {
                    let template = temp.cloneNode(true).content;
                    if (typeof TelefinConfig.notificationType.values[key] !== 'string') {
                        template = temp_without_textarea.cloneNode(true).content;
                    }
                    const name = template.querySelector("[data-name=notificationTypeName]");
                    const value = template.querySelector("[data-name=notificationTypeValue]");

                    if (typeof TelefinConfig.notificationType.values[key] !== 'string') {
                        name.innerText = TelefinConfig.notificationType.values[key][0];
                    } else {
                        name.innerText = TelefinConfig.notificationType.values[key];
                        const textarea = template.querySelector('[data-name="txtTemplate"]');
                        textarea.value = userConfig === null ? this.defaultMessages[key] : userConfig[key + 'StringMessage'];
                        textarea.dataset.value = key;
                    }
                    value.dataset.value = key;
                    if (userConfig === null) {
                        value.checked = false;
                    } else {
                        value.checked = userConfig[key] === true;
                    }
                    container.appendChild(template);

                    // Notification subtypes
                    if (typeof TelefinConfig.notificationType.values[key] !== 'string') {
                        for (const subtype of TelefinConfig.notificationType.values[key].slice(1)) {
                            template = subtemp.cloneNode(true).content;
                            const name = template.querySelector("[data-name=notificationSubtypeName]");
                            const value = template.querySelector("[data-name=notificationSubtypeValue]");
                            const textarea = template.querySelector('[data-name="txtTemplate"]');

                            name.innerText = subtype;
                            const subkey = key + subtype.replace(/\s/g, '');
                            value.dataset.value = subkey;
                            textarea.dataset.value = subkey;
                            if (userConfig === null) {
                                value.checked = false;
                                textarea.value = this.defaultMessages[subkey];
                            } else {
                                value.checked = userConfig[subkey] === true;
                                textarea.value = userConfig[subkey + 'StringMessage'];
                            }
                            container.appendChild(template);
                        }
                    }
                }
            },

            saveNotificationTypes: function (userConfig) {
                const notificationTypeKeys = Object.keys(TelefinConfig.notificationType.values).sort();
                for (const key of notificationTypeKeys) {
                    userConfig[key] = document.querySelector(`[data-name=notificationTypeValue][data-value=${key}]`).checked;
                    if (typeof TelefinConfig.notificationType.values[key] === 'string') {
                        userConfig[key + 'StringMessage'] = document.querySelector(`[data-name=txtTemplate][data-value=${key}]`).value;
                    }

                    // Notification subtypes
                    if (typeof TelefinConfig.notificationType.values[key] !== 'string') {
                        for (const subtype of TelefinConfig.notificationType.values[key].slice(1)) {
                            const subkey = key + subtype.replace(/\s/g, '');
                            userConfig[subkey] = document.querySelector(`[data-name=notificationSubtypeValue][data-value=${subkey}]`).checked;
                            userConfig[subkey + 'StringMessage'] = document.querySelector(`[data-name=txtTemplate][data-value=${subkey}]`).value;
                        }
                    }
                }
            }
        },

        user: {
            loadUsers: async function () {
                const users = await window.ApiClient.getUsers();
                const selectElement = document.getElementById("userToConfigure");
                selectElement.innerHTML = '';
                for (const user of users) {
                    const option = document.createElement('option');
                    option.value = user.Id;
                    option.textContent = user.Name;
                    selectElement.appendChild(option);
                }
            },
            getSelectedUserId: function () {
                const userId = document.getElementById("userToConfigure").value;
                return userId;
            }
        },

        init: async function () {
            // Load users and pick first one by default
            await this.user.loadUsers();
            const select = document.getElementById('userToConfigure');
            if (select && select.options.length > 0 && !select.value) {
                select.selectedIndex = 0;
            }

            this.loadConfig();

            document.getElementById('userToConfigure').addEventListener('change', this.loadConfig);
            document.getElementById('testButton').addEventListener('click', this.testBotConfig);
            document.getElementById('saveButton').addEventListener('click', this.saveConfig);

            // Single delegated handler for template edit/reset
            document.body.addEventListener('click', (event) => {
                const editButton = event.target.closest('.edit-template-button');
                const resetButton = event.target.closest('.reset-template-button');

                if (editButton) {
                    event.preventDefault();

                    // New layout: buttons live inside .telefin-item, editor wrapper is .telefin-editor
                    const item = editButton.closest('.telefin-item');
                    if (!item) return;

                    const editor = item.querySelector('.telefin-editor');
                    const textarea = item.querySelector('textarea[data-name="txtTemplate"]');
                    const reset = item.querySelector('.reset-template-button');

                    if (!textarea) return;

                    const isHidden = textarea.style.display === 'none' || textarea.style.display === '';

                    // Toggle editor + textarea together
                    textarea.style.display = isHidden ? 'block' : 'none';
                    if (editor) editor.style.display = isHidden ? 'block' : 'none';

                    // Show reset only when customizing is open
                    if (reset) reset.style.display = isHidden ? 'inline-block' : 'none';
                }

                if (resetButton) {
                    event.preventDefault();

                    // New layout: same idea, find the containing item
                    const item = resetButton.closest('.telefin-item');
                    if (!item) return;

                    const editor = item.querySelector('.telefin-editor');
                    const textarea = item.querySelector('textarea[data-name="txtTemplate"]');
                    if (!textarea) return;

                    const key = textarea.dataset.value;
                    textarea.value = TelefinConfig.notificationType.defaultMessages[key];

                    // Optional: keep the editor open after reset, so user sees it changed
                    textarea.style.display = 'block';
                    if (editor) editor.style.display = 'block';
                    resetButton.style.display = 'inline-block';
                }
            });
        },

        loadConfig: function () {
            Dashboard.showLoadingMsg();
            ApiClient.getPluginConfiguration(TelefinConfig.pluginUniqueId).then(function (config) {
                document.querySelector('#EnablePlugin').checked = config.EnablePlugin;
                const userConfig = config.UserConfigurations.find(x => x.UserId === TelefinConfig.user.getSelectedUserId());
                if (userConfig) {
                    document.querySelector('#ServerUrl').value = config.ServerUrl;
                    document.querySelector('#BotToken').value = userConfig.BotToken;
                    document.querySelector('#ChatId').value = userConfig.ChatId;
                    document.querySelector('#ThreadId').value = userConfig.ThreadId;
                    document.querySelector('#EnableUser').checked = userConfig.EnableUser;
                    document.querySelector('#SilentNotification').checked = userConfig.SilentNotification;
                    document.querySelector('#DoNotMentionOwnActivities').checked = userConfig.DoNotMentionOwnActivities;
                    TelefinConfig.notificationType.loadNotificationTypes(userConfig);
                } else {
                    document.querySelector('#ServerUrl').value = config.ServerUrl;
                    document.querySelector('#BotToken').value = '';
                    document.querySelector('#ChatId').value = '';
                    document.querySelector('#ThreadId').value = '';
                    document.querySelector('#EnableUser').checked = false;
                    document.querySelector('#SilentNotification').checked = false;
                    document.querySelector('#DoNotMentionOwnActivities').checked = false;
                    TelefinConfig.notificationType.loadNotificationTypes(null);
                }
                Dashboard.hideLoadingMsg();
            });
        },

        saveConfig: function (e = null) {
            if (e) {
                e.preventDefault();
            }
            return new Promise((resolve, reject) => {
                Dashboard.showLoadingMsg();
                ApiClient.getPluginConfiguration(TelefinConfig.pluginUniqueId).then(function (config) {
                    config.EnablePlugin = document.querySelector('#EnablePlugin').checked;
                    const userConfig = config.UserConfigurations.find(x => x.UserId === TelefinConfig.user.getSelectedUserId());
                    if (userConfig) {
                        config.ServerUrl = document.querySelector('#ServerUrl').value;
                        userConfig.BotToken = document.querySelector('#BotToken').value;
                        userConfig.ChatId = document.querySelector('#ChatId').value;
                        userConfig.ThreadId = document.querySelector('#ThreadId').value;
                        userConfig.EnableUser = document.querySelector('#EnableUser').checked;
                        userConfig.SilentNotification = document.querySelector('#SilentNotification').checked;
                        userConfig.DoNotMentionOwnActivities = document.querySelector('#DoNotMentionOwnActivities').checked;
                        TelefinConfig.notificationType.saveNotificationTypes(userConfig);
                    } else {
                        config.ServerUrl = document.querySelector('#ServerUrl').value;
                        config.UserConfigurations.push({
                            UserId: TelefinConfig.user.getSelectedUserId(),
                            UserName: document.querySelector('#userToConfigure').selectedOptions[0].text,
                            BotToken: document.querySelector('#BotToken').value,
                            ChatId: document.querySelector('#ChatId').value,
                            ThreadId: document.querySelector('#ThreadId').value,
                            EnableUser: document.querySelector('#EnableUser').checked,
                            SilentNotification: document.querySelector('#SilentNotification').checked,
                            DoNotMentionOwnActivities: document.querySelector('#DoNotMentionOwnActivities').checked,
                        });
                        TelefinConfig.notificationType.saveNotificationTypes(config.UserConfigurations.find(x => x.UserId === TelefinConfig.user.getSelectedUserId()));
                    }
                    ApiClient.updatePluginConfiguration(TelefinConfig.pluginUniqueId, config).then(function (result) {
                        Dashboard.processPluginConfigurationUpdateResult(result);
                        resolve(result);
                    }).catch(reject);
                }).catch(reject);
            });
        },

        testBotConfig: function () {
            var button = this;
            button.disabled = true;
            TelefinConfig.saveConfig()
                .then(function () {
                    return ApiClient.getPluginConfiguration(TelefinConfig.pluginUniqueId);
                })
                .then(function (config) {
                    const userConfig = config.UserConfigurations.find(x => x.UserId === TelefinConfig.user.getSelectedUserId());
                    var threadId = userConfig.ThreadId;
                    if (threadId === '') {
                        threadId = null;
                    }
                    const params = {
                        botToken: userConfig.BotToken,
                        chatId: userConfig.ChatId,
                        threadId: threadId,
                    };
                    const url = new URL('/TelefinApi/TestNotifier', window.location.origin);
                    Object.keys(params).forEach(key => url.searchParams.append(key, params[key]));
                    return fetch(url);
                })
                .then(function (response) {
                    Dashboard.hideLoadingMsg();
                    if (!response.ok) {
                        throw new Error('Error while sending the test message to the telegram bot');
                    }
                    button.style.backgroundColor = 'green';
                    button.style.color = 'white';
                    button.textContent = 'Test passed';
                })
                .catch(function () {
                    button.style.backgroundColor = 'red';
                    button.style.color = 'white';
                    button.textContent = 'Test failed';
                })
                .finally(function () {
                    setTimeout(function () {
                        button.disabled = false;
                        button.style.color = '';
                        button.style.backgroundColor = '';
                        button.textContent = 'Test bot configuration';
                    }, 2000);
                });
        }
    };

    view.addEventListener('viewshow', async function () {
        await TelefinConfig.init();
    });
}