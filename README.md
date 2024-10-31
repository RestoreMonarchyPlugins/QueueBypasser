# Queue Bypasser
Allow players with permission to join the server even when it's full.

## Features
- Bypass queue for admins (players with admin blue hammer)
- Bypass queue by permission (permission is: `queuebypasser`)
- Option to bypass queue for new players (players who join the server for the first time)
- Option to bypass queue for relogging players (players who disconnected from the server and reconnected within a certain time)
- Great no P2W feature for servers!

### How it works?
It doesn't kick any players from the server, instead it just allows the player to join the server even when it's full and there are other players in the queue.  
For example the server is **24/24**, when a player with bypass permission joins the server, it will be **25/24**.

## Credits
**kulkaGM** for creating this plugin.

## Permissions
```xml
<Permission Cooldown="0">queuebypasser</Permission>
```

## Configuration
```xml
<?xml version="1.0" encoding="utf-8"?>
<QueueBypasserConfiguration xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <IsLoggingEnabled>true</IsLoggingEnabled>
  <ShouldBypassQueueForAdmins>true</ShouldBypassQueueForAdmins>
  <ShouldBypassQueueForNewPlayers>false</ShouldBypassQueueForNewPlayers>
  <RelogBypassSettings Enabled="false" Cooldown="900" />
</QueueBypasserConfiguration>
```