FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
ARG BUILD_CONFIGURATION=Release
COPY ["../../../CQRS.Core", "/src/CQRS.Core"]
COPY ["../../../SocialMedia.Post", "/src/SocialMedia.Post"]
WORKDIR "/src/SocialMedia.Post/Post.Command/SocialMedia.Post.Command.Api"
RUN dotnet publish "./SocialMedia.Post.Command.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialMedia.Post.Command.Api.dll"]