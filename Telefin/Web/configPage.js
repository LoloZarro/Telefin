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

            defaultMessages: {},

            loadDefaultMessages: async function () {
                const response = await fetch('/TelefinApi/DefaultMessages', {
                    method: 'GET',
                    headers: { 'Accept': 'application/json' }
                });

                if (!response.ok) {
                    console.error('Failed to load default messages', response.status, await response.text());
                    return {};
                }

                return await response.json();
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

            this.notificationType.defaultMessages = await this.notificationType.loadDefaultMessages() ?? {};

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
                    const url = new URL('/TelefinApi/SmokeTest', window.location.origin);
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