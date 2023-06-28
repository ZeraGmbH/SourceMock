# Set up project

### Recommended extensions for vscode

The `.vscode/extensions.json` contains a list of recommended extensions for this project. If you havn't already installed them, vscode should alert you on startup.

### git settings

Load recommended git settings with `git config --local include.path ../.gitconfig`. You can still commit failing builds using the `--no-verify` option (you shouldn't, though). These settings crrently include:

- git hooks

## First start and SSL certificates

The web server uses a self signed SSL certificate. If this certificate is not trusted, some web browsers will not do a propper HTTPS redirection, which may lead to an API call to be blocked because it requires HTTPS. In this case, you need to go manually to the swagger page (chage the port to the one specified in `appsettings.json` and add the `https://`-prefix).

# CI/CD

## Builds

There are builds for MS Windows as well as linux, each in a 64 bit variant. Both builds are made uploaded as build artifacts.

## Tests

### Unit tests

The nunit unit tests are run by the CD-pipeline. 

### Integration tests

The application is started with the default production appsettings and tested using [newman](https://learning.postman.com/docs/collections/using-newman-cli/command-line-integration-with-newman/). New tests can be designed using [postman](https://www.postman.com/).
