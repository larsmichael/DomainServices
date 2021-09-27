# How to push a nuget package?
To push a new version of the nuget package, do the following:

1. In the `.csproj` file, modify the version number according to [semantic versioning](https://semver.org/).
2. Update the release notes (if any). 
3. Run `PackAndPush.bat`.
4. Commit and push the modifications to git repository with a comment stating the version number and the reason (for example “2.2.4 bug fix”).

