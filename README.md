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

----------------
## Using Template Variable

For using template variable, we need to perform two steps:

#### 1. Define Template within the secret content: 

We can use ``{{<TEMPLATE_VARIABLE>}}`` to define a template variable in the secret document.

#### 2. Define Template within the `.env` file, for this we can define keys with the following format:

We can define two types of template variable:
 - `DATA__<KEY>`: This is a default template variable, and used for specifying general data
 - `DATA_<ENVIRONMENT>__<KEY>`: This is a template variable defined for specific environment in case of needs. 
                                If this key is provided, it has higher priority; 
                                Otherwise, the system will fall back to use the default template key.

#### SAMPLE:

```dotenv
DATA__SQL_USERNAME=foo
DATA__SQL_PASSWORD=bar
DATA_PROD__SQL_PASSWORD=baz
```