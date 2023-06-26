# Source Mock

A mock of a ZERA source, containing an API specification.

## Set up project

### Recommended extensions for vscode

The `.vscode/extensions.json` contains a list of recommended extensions for this project. If you havn't already installed them, vscode should alert you on startup.

### git settings

Load recommended git settings with `git config --local include.path ../.gitconfig`. You can still commit failing builds using the `--no-verify` option (you shouldn't, though). These settings crrently include:

- git hooks

## First start and SSL certificates

The web server uses a self signed SSL certificate. If this certificate is not trusted, some web browsers will not do a propper HTTPS redirection, which may lead to an API call to be blocked because it requires HTTPS. In this case, you need to go manually to the swagger page (chage the port to the one specified in `appsettings.json` and add the `https://`-prefix).
