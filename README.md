Create an `.env` file within the `SecretSwitcher` project root.

`.env` content:
```dotenv
SWITCHER_BASE_ADDRESS=<YOUR PROJECT [GROUP] SOLUTION PATH>
SWITCHER_MONGODB_CONNECTION_STRING=<MONGODB SECRECT MANAGER CONNECTION STRING>
SWITCHER_DATABASE_NAME=<MONGODB SECRET MANAGER DATABASE>
```

Change `.env` file properties to:
 - Build Action: `Content`
 - Copy to output directory: `Copy if newer`
