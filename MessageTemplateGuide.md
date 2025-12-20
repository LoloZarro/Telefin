# **Telefin – Notification Template Placeholders**

Telefin allows you to customize the text sent for each notification type.
Inside your custom message, you can use **placeholders** such as `{username}` or `{itemTitle}`.
These will automatically be replaced with the correct values when a notification is sent.

Below you will find the complete list of placeholders available for each notification type.

---

# **Playback Notifications**

Used in:

* **Playback Start**
* **Playback Stop**
* **Playback Progress**

### **Available placeholders**

| Placeholder       | Meaning                                       |
| ----------------- | --------------------------------------------- |
| `{username}`      | Name of the user watching                     |
| `{deviceName}`    | Device being used                             |
| `{playMethod}`    | Playback method (DirectPlay, Transcode, etc.) |
| `{itemTitle}`     | Title of the item (movie/episode/etc.)        |
| `{itemYear}`      | Year of release                               |
| `{mediaType}`     | Media type (Movie, Episode, etc.)             |
| `{itemOverview}`  | Description / overview                        |
| `{itemGenres}`    | Genres                                        |
| `{itemDuration}`  | Duration in human-readable form               |
| `{seriesTitle}`   | Series name (for episodes/seasons)            |
| `{seasonNumber}`  | Season number (episodes/seasons)              |
| `{episodeNumber}` | Episode number (episodes)                     |

---

# **Library Notifications**

Used in:

* **Item Added**
* **Item Deleted**

### **Available placeholders**

| Placeholder       | Meaning                                   |
| ----------------- | ----------------------------------------- |
| `{itemTitle}`     | Title of the item                         |
| `{itemYear}`      | Year of release                           |
| `{mediaType}`     | Media type (Movie, Series, Episode, etc.) |
| `{itemOverview}`  | Description / overview                    |
| `{itemGenres}`    | Genres                                    |
| `{itemDuration}`  | Duration                                  |
| `{seriesTitle}`   | Series title (for seasons/episodes)       |
| `{seasonNumber}`  | Season number                             |
| `{episodeNumber}` | Episode number                            |

---

# **Authentication Notifications**

## Authentication Failure

### **Available placeholders**

| Placeholder    | Meaning                             |
| -------------- | ----------------------------------- |
| `{deviceName}` | Device used during the failed login |
| `{username}`   | Username that attempted login       |

---

## Authentication Success

### **Available placeholders**

| Placeholder      | Meaning                    |
| ---------------- | -------------------------- |
| `{deviceName}`   | Device used during login   |
| `{username}`     | Logged-in username         |
| `{serverName}`   | Server name                |
| `{lastActivity}` | Timestamp of last activity |

---

# **Plugin Notifications**

## Plugin Installed / Installing / Updated / Installation Cancelled

### **Available placeholders**

| Placeholder                | Meaning             |
| -------------------------- | ------------------- |
| `{itemName}`               | Plugin name         |
| `{itemVersion}`            | Version             |
| `{itemUrl}`                | Download/source URL |
| `{itemPackageName}`        | Package name        |
| `{itemPackageDescription}` | Short description   |
| `{itemPackageOverview}`    | Long description    |
| `{itemPackageCategory}`    | Category            |

---

## Plugin Installation Failed

### **Available placeholders**

| Placeholder                | Meaning             |
| -------------------------- | ------------------- |
| `{errorMessage}`           | Error message       |
| `{itemName}`               | Plugin name         |
| `{itemVersion}`            | Version             |
| `{itemUrl}`                | Download/source URL |
| `{itemPackageName}`        | Package name        |
| `{itemPackageDescription}` | Description         |
| `{itemPackageOverview}`    | Long description    |
| `{itemPackageCategory}`    | Category            |

---

## Plugin Uninstalled

### **Available placeholders**

| Placeholder                | Meaning                     |
| -------------------------- | --------------------------- |
| `{itemName}`               | Plugin name                 |
| `{itemVersion}`            | Version                     |
| `{itemDescription}`        | Description                 |
| `{itemPackageDescription}` | Description (same as above) |
| `{itemStatus}`             | Final status                |

---

# **Subtitle Download Failure**

### **Available placeholders**

| Placeholder          | Meaning                       |
| -------------------- | ----------------------------- |
| `{subtitleProvider}` | Name of the subtitle provider |
| `{errorMessage}`     | Error message                 |
| `{itemTitle}`        | Item name                     |
| `{itemYear}`         | Year                          |
| `{mediaType}`        | Media type                    |
| `{itemOverview}`     | Description                   |
| `{itemGenres}`       | Genres                        |
| `{itemDuration}`     | Duration                      |
| `{seriesTitle}`      | Series title                  |
| `{seasonNumber}`     | Season number                 |
| `{episodeNumber}`    | Episode number                |

---

# **Task Completed**

### **Available placeholders**

### Task result

| Placeholder         | Meaning                |
| ------------------- | ---------------------- |
| `{status}`          | Completion status      |
| `{name}`            | Task result name       |
| `{key}`             | Task key               |
| `{id}`              | Task result id         |
| `{error}`           | Error message (if any) |
| `{fullError}`       | Long error message     |
| `{startTimeUtc}`    | Start time (UTC)       |
| `{endTimeUtc}`      | End time (UTC)         |
| `{durationRaw}`     | Raw duration format    |
| `{durationSeconds}` | Duration in seconds    |
| `{durationMinutes}` | Duration in minutes    |

### Task metadata

| Placeholder         | Meaning          |
| ------------------- | ---------------- |
| `{taskName}`        | Task name        |
| `{taskDescription}` | Task description |
| `{taskCategory}`    | Category         |
| `{taskId}`          | ID               |
| `{taskState}`       | Current state    |
| `{taskProgress}`    | Progress (%)     |

---

# **User Notifications**

Used in:

* **User Created**
* **User Deleted**
* **User Updated**
* **User Password Changed**
* **User Locked Out**

### **Available placeholders**

### Identity

| Placeholder    | Meaning             |
| -------------- | ------------------- |
| `{userId}`     | User GUID           |
| `{username}`   | Username            |
| `{internalId}` | Internal numeric ID |

### Login / Activity

| Placeholder                    | Meaning                     |
| ------------------------------ | --------------------------- |
| `{authProvider}`               | Authentication provider     |
| `{passwordResetProvider}`      | Password reset provider     |
| `{invalidLoginAttempts}`       | Failed login attempts       |
| `{lastLogin}`                  | Last login date             |
| `{lastActivity}`               | Last activity date          |
| `{loginAttemptsBeforeLockout}` | Max attempts before lockout |

### Password & Access

| Placeholder                    | Meaning                         |
| ------------------------------ | ------------------------------- |
| `{mustUpdatePassword}`         | Requires password change        |
| `{hasPassword}`                | Whether the user has a password |
| `{enableLocalPassword}`        | Local password enabled          |
| `{enableAutoLogin}`            | Auto login enabled              |
| `{enableUserPreferenceAccess}` | Can change preferences          |

### Profile

| Placeholder        | Meaning            |
| ------------------ | ------------------ |
| `{profileImage}`   | Profile image path |
| `{castReceiverId}` | Cast receiver ID   |

### Preferences

| Placeholder                    | Meaning                      |
| ------------------------------ | ---------------------------- |
| `{audioLang}`                  | Audio language               |
| `{subtitleLang}`               | Subtitle language            |
| `{subtitleMode}`               | Subtitle mode                |
| `{playDefaultAudio}`           | Play default audio           |
| `{rememberAudioSelections}`    | Remember audio selections    |
| `{rememberSubtitleSelections}` | Remember subtitle selections |

### UI / Library preferences

| Placeholder                | Meaning                 |
| -------------------------- | ----------------------- |
| `{displayMissingEpisodes}` | Show missing episodes   |
| `{displayCollectionsView}` | Show collections        |
| `{hidePlayedInLatest}`     | Hide played in "Latest" |

### Parental Controls & Bitrate

| Placeholder                   | Meaning            |
| ----------------------------- | ------------------ |
| `{maxParentalRatingScore}`    | Max parental score |
| `{maxParentalRatingSubScore}` | Max sub-score      |
| `{remoteClientBitrateLimit}`  | Bitrate limit      |

### Sync & Sessions

| Placeholder           | Meaning              |
| --------------------- | -------------------- |
| `{maxActiveSessions}` | Max allowed sessions |
| `{syncPlayAccess}`    | SyncPlay permissions |

### Misc

| Placeholder    | Meaning        |
| -------------- | -------------- |
| `{rowVersion}` | Version number |

---

# **Notes for Writing Templates**

* Placeholders are **case-sensitive**.
* If you use a placeholder not supported for that notification type, it will appear unchanged.
* You can include **emoji**, **line breaks**, and any text you want around the placeholders.
* Depeding on the media type, some placeholder will not contain certain values (e.g. SeasonNumber for Movies)

---
