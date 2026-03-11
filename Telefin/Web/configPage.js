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
                const subtemp = document.querySelector("#template-notification-subtype");
                const container = document.querySelector("[data-name=notificationTypeContainer]");
                container.innerHTML = '';

                const notificationTypeKeys = Object.keys(TelefinConfig.notificationType.values).sort();
                for (const key of notificationTypeKeys) {
                    const def = TelefinConfig.notificationType.values[key];

                    const fragment = temp.cloneNode(true).content;
                    const name = fragment.querySelector("[data-name=notificationTypeName]");
                    const value = fragment.querySelector("[data-name=notificationTypeValue]");
                    const textarea = fragment.querySelector('[data-name="txtTemplate"]');
                    const editor = fragment.querySelector('.telefin-editor');
                    const editBtn = fragment.querySelector('.edit-template-button');
                    const resetBtn = fragment.querySelector('.reset-template-button');
                    const toggleBtn = fragment.querySelector('.toggle-subtypes-button');
                    const children = fragment.querySelector('[data-name="subtypesContainer"]');

                    value.dataset.value = key;
                    value.checked = userConfig ? (userConfig[key] === true) : false;

                    // Leaf notification types (support templates)
                    if (typeof def === 'string') {
                        name.innerText = def;

                        textarea.value = userConfig === null ? this.defaultMessages[key] : userConfig[key + 'StringMessage'];
                        textarea.dataset.value = key;

                        // No subtypes for leaf types
                        if (toggleBtn) toggleBtn.style.display = 'none';
                        if (children) children.style.display = 'none';
                    }
                    // Top-level groups (expandable list of subtypes)
                    else {
                        name.innerText = def[0];

                        // Hide template controls for group rows
                        if (editBtn) editBtn.style.display = 'none';
                        if (resetBtn) resetBtn.style.display = 'none';
                        if (editor) editor.style.display = 'none';
                        if (textarea) textarea.style.display = 'none';

                        // Setup expand/collapse toggle (collapsed by default)
                        if (toggleBtn) {
                            toggleBtn.style.display = 'inline-block';
                            const s = toggleBtn.querySelector('span');
                        }

                        // Build subtypes into the group's children container
                        if (children) {
                            children.style.display = 'none';

                            for (const subtype of def.slice(1)) {
                                const subFrag = subtemp.cloneNode(true).content;
                                const subName = subFrag.querySelector("[data-name=notificationSubtypeName]");
                                const subValue = subFrag.querySelector("[data-name=notificationSubtypeValue]");
                                const subTextarea = subFrag.querySelector('[data-name="txtTemplate"]');

                                subName.innerText = subtype;

                                const subkey = key + subtype.replace(/\s/g, '');
                                subValue.dataset.value = subkey;
                                subTextarea.dataset.value = subkey;

                                if (userConfig === null) {
                                    subValue.checked = false;
                                    subTextarea.value = this.defaultMessages[subkey];
                                } else {
                                    subValue.checked = userConfig[subkey] === true;
                                    subTextarea.value = userConfig[subkey + 'StringMessage'];
                                }

                                children.appendChild(subFrag);
                            }
                        }
                    }

                    container.appendChild(fragment);
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

        users: {
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
            await this.users.loadUsers();

            //const select = document.getElementById('userToConfigure');
            //if (select && select.options.length > 0 && !select.value) {
            //    select.selectedIndex = 0;
            //}

            this.notificationType.defaultMessages = await this.notificationType.loadDefaultMessages() ?? {};

            this.loadConfig();

            document.getElementById('userToConfigure').addEventListener('change', this.loadConfig);
            document.getElementById('testButton').addEventListener('click', this.testBotConfig);
            document.getElementById('saveButton').addEventListener('click', this.saveConfig);
            document.getElementById('addChatButton').addEventListener('click', () => {
                this.configuredChats.addRow({ ChatId: '', ThreadId: '' });
            });

            document.getElementById('ConfiguredChatsContainer').addEventListener('click', (event) => {
                const removeBtn = event.target.closest('.remove-chat-button');
                if (!removeBtn) return;
                event.preventDefault();
                const row = removeBtn.closest('.configured-chat-row');
                if (row) row.remove();
                TelefinConfig.configuredChats.updateRemoveButtons();
            });

            // Single delegated handler for template edit/reset
            document.body.addEventListener('click', (event) => {
                const toggleButton = event.target.closest('.toggle-subtypes-button');
                const editButton = event.target.closest('.edit-template-button');
                const resetButton = event.target.closest('.reset-template-button');

                if (toggleButton) {
                    event.preventDefault();

                    const item = toggleButton.closest('.telefin-item');
                    if (!item) return;

                    const children = item.querySelector('.telefin-children');
                    if (!children) return;

                    const isHidden = children.style.display === 'none' || children.style.display === '';
                    children.style.display = isHidden ? 'block' : 'none';

                    const expandIcon = toggleButton.querySelector('.toggle-icon-expand');
                    const collapseIcon = toggleButton.querySelector('.toggle-icon-collapse');

                    if (expandIcon && collapseIcon) {
                        expandIcon.style.display = isHidden ? 'none' : 'inline';
                        collapseIcon.style.display = isHidden ? 'inline' : 'none';
                    }

                    const s = toggleButton.querySelector('.toogle-subtypes-button-text');
                    if (s) s.textContent = isHidden ? 'Collapse' : 'Expand';
                    return;
                }

                if (editButton) {
                    event.preventDefault();

                    // Buttons live inside .telefin-item, editor wrapper is .telefin-editor
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

                    const item = resetButton.closest('.telefin-item');
                    if (!item) return;

                    const editor = item.querySelector('.telefin-editor');
                    const textarea = item.querySelector('textarea[data-name="txtTemplate"]');
                    if (!textarea) return;

                    const key = textarea.dataset.value;
                    textarea.value = TelefinConfig.notificationType.defaultMessages[key];

                    // Keep the editor open after reset, so user sees it changed
                    textarea.style.display = 'block';
                    if (editor) editor.style.display = 'block';
                    resetButton.style.display = 'inline-block';
                }
            });
        },

        configuredChats: {
            container: function () {
                return document.getElementById('ConfiguredChatsContainer');
            },
            addRow: function (chat) {
                const template = document.getElementById('template-configured-chat');
                const fragment = template.cloneNode(true).content;
                const row = fragment.querySelector('.configured-chat-row');

                row.querySelector('[data-name="ChatId"]').value = chat?.ChatId ?? '';
                row.querySelector('[data-name="ThreadId"]').value = chat?.ThreadId ?? '';

                this.container().appendChild(fragment);
                this.updateRemoveButtons();
            },
            setAll: function (chats) {
                const c = this.container();
                c.innerHTML = '';

                const list = Array.isArray(chats) ? chats : [];
                if (list.length === 0) {
                    this.addRow({ ChatId: '', ThreadId: '' });
                    return;
                }

                for (const chat of list) {
                    this.addRow(chat);
                }
            },
            getAll: function () {
                const rows = Array.from(this.container().querySelectorAll('.configured-chat-row'));
                const chats = rows.map(row => {
                    const chatId = row.querySelector('[data-name="ChatId"]').value?.trim() ?? '';
                    const threadId = row.querySelector('[data-name="ThreadId"]').value?.trim() ?? '';
                    return { ChatId: chatId, ThreadId: threadId };
                });

                // Only persist rows with a ChatId
                return chats.filter(x => x.ChatId.length > 0);
            },
            updateRemoveButtons: function () {
                const rows = Array.from(this.container().querySelectorAll('.configured-chat-row'));
                const showRemove = rows.length > 1;
                for (const row of rows) {
                    const btn = row.querySelector('.remove-chat-button');
                    if (btn) btn.style.display = showRemove ? 'inline-flex' : 'none';
                }
            }
        },

        loadConfig: function () {
            Dashboard.showLoadingMsg();
            ApiClient.getPluginConfiguration(TelefinConfig.pluginUniqueId).then(function (config) {
                document.querySelector('#EnablePlugin').checked = config.EnablePlugin;
                document.querySelector('#ServerUrl').value = config.ServerUrl;

                const mw = parseInt(config.MetadataWaitMultiplier ?? 1, 10);
                document.querySelector('#MetadataWaitMultiplier').value = Number.isFinite(mw) ? Math.min(100, Math.max(1, mw)) : 1;

                const debounceMs = parseInt(config.PlaybackStartDebounceMs ?? 0, 10);
                document.querySelector('#PlaybackStartDebounceMs').value = Number.isFinite(debounceMs) ? Math.min(60000, Math.max(0, debounceMs)) : 0;

                document.querySelector('#SuppressMovedMediaNotifications').checked = !!config.SuppressMovedMediaNotifications;

                const userConfig = config.UserConfigurations.find(x => x.UserId === TelefinConfig.users.getSelectedUserId());
                if (userConfig) {
                    document.querySelector('#BotToken').value = userConfig.BotToken;
                    TelefinConfig.configuredChats.setAll(userConfig.ConfiguredChats);
                    document.querySelector('#EnableUser').checked = userConfig.EnableUser;
                    document.querySelector('#SilentNotification').checked = userConfig.SilentNotification;
                    document.querySelector('#DoNotMentionOwnActivities').checked = userConfig.DoNotMentionOwnActivities;
                    TelefinConfig.notificationType.loadNotificationTypes(userConfig);
                } else {
                    document.querySelector('#BotToken').value = '';
                    TelefinConfig.configuredChats.setAll([]);
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
                    config.ServerUrl = document.querySelector('#ServerUrl').value;

                    const mw = parseInt(document.querySelector('#MetadataWaitMultiplier').value ?? '1', 10);
                    config.MetadataWaitMultiplier = Number.isFinite(mw) ? Math.min(100, Math.max(1, mw)) : 1;

                    const debounceMs = parseInt(document.querySelector('#PlaybackStartDebounceMs').value ?? '0', 10);
                    config.PlaybackStartDebounceMs = Number.isFinite(debounceMs) ? Math.min(60000, Math.max(0, debounceMs)) : 0;

                    config.SuppressMovedMediaNotifications = document.querySelector('#SuppressMovedMediaNotifications').checked;

                    const userConfig = config.UserConfigurations.find(x => x.UserId === TelefinConfig.users.getSelectedUserId());
                    if (userConfig) {
                        userConfig.BotToken = document.querySelector('#BotToken').value;
                        userConfig.ConfiguredChats = TelefinConfig.configuredChats.getAll();
                        userConfig.EnableUser = document.querySelector('#EnableUser').checked;
                        userConfig.SilentNotification = document.querySelector('#SilentNotification').checked;
                        userConfig.DoNotMentionOwnActivities = document.querySelector('#DoNotMentionOwnActivities').checked;
                        TelefinConfig.notificationType.saveNotificationTypes(userConfig);
                    } else {
                        config.UserConfigurations.push({
                            UserId: TelefinConfig.users.getSelectedUserId(),
                            UserName: document.querySelector('#userToConfigure').selectedOptions[0].text,
                            BotToken: document.querySelector('#BotToken').value,
                            ConfiguredChats: TelefinConfig.configuredChats.getAll(),
                            EnableUser: document.querySelector('#EnableUser').checked,
                            SilentNotification: document.querySelector('#SilentNotification').checked,
                            DoNotMentionOwnActivities: document.querySelector('#DoNotMentionOwnActivities').checked,
                        });
                        TelefinConfig.notificationType.saveNotificationTypes(config.UserConfigurations.find(x => x.UserId === TelefinConfig.users.getSelectedUserId()));
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
                    const userConfig = config.UserConfigurations.find(x => x.UserId === TelefinConfig.users.getSelectedUserId());
                    const payload = {
                        botToken: userConfig.BotToken,
                        configuredChats: userConfig.ConfiguredChats ?? []
                    };
                    return fetch('/TelefinApi/SmokeTest', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'Accept': 'application/json'
                        },
                        body: JSON.stringify(payload)
                    });
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