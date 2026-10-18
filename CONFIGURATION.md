# Configuration Setup

## Important: Sensitive Data Protection

This project uses `.gitignore` to prevent sensitive configuration files from being committed to version control. The following files are ignored:

- `appsettings.json` - Contains API keys, connection strings, and secrets
- `.kiro/` - Kiro AI IDE configuration and workspace files
- `*.db-shm`, `*.db-wal` - SQLite database temporary files

## Setting Up Your Local Configuration

1. **Copy the example configuration file:**
   ```powershell
   Copy-Item CareerPath/appsettings.json.example CareerPath/appsettings.json
   ```

2. **Update the configuration with your actual values:**
   
   Edit `CareerPath/appsettings.json` and replace the placeholder values:

   - **Connection Strings**: Update with your actual SQL Server credentials
   - **JWT Secret Key**: Generate a secure random key (at least 32 characters)
   - **Email Settings**: Add your SMTP credentials (for Gmail, use an App Password)
   - **API Keys**: Add your Jooble API key and any other external service keys

3. **Never commit the actual `appsettings.json` file**
   
   The file is already in `.gitignore`, but double-check before committing:
   ```powershell
   git status
   ```

## Configuration Values Needed

### Database Connection Strings
- Local development: Uses SQL Server LocalDB (default in example)
- Remote/Production: Requires actual server credentials

### JWT Authentication
- `SecretKey`: A secure random string (minimum 32 characters)
- `Issuer`: "CareerPath" (default)
- `Audience`: "CareerPathAPI" (default)

### Email Service (SMTP)
- SMTP Server: smtp.gmail.com (or your provider)
- Port: 465 (SSL) or 587 (TLS)
- Username: Your email address
- Password: App-specific password (recommended for Gmail)

### External APIs
- **Jooble API**: Register at [Jooble API](https://jooble.org/api/about) to get your API key

## Security Best Practices

1. **Never share your `appsettings.json` file**
2. **Use different credentials for development and production**
3. **Rotate API keys and passwords regularly**
4. **Use environment variables or Azure Key Vault in production**
5. **Keep the `.example` file updated when adding new configuration keys**

## Production Deployment

For production environments (like Railway), use environment variables instead of `appsettings.json`:

- Set configuration values in your hosting platform's environment variables
- ASP.NET Core will automatically override `appsettings.json` values with environment variables

Example environment variable naming:
- `ConnectionStrings__RemoteConnection`
- `Jwt__SecretKey`
- `EmailSettings__Password`
- `Sources__Jooble_API_Key`
