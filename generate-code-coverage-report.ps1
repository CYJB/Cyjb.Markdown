dotnet test --no-build
dotnet reportgenerator -reports:.\TestMarkdown\coverage.cobertura.xml -targetdir:.\CodeCoverage
