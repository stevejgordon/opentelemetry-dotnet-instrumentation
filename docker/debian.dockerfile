FROM mcr.microsoft.com/dotnet/sdk:8.0.403-bookworm-slim@sha256:cab0284cce7bc26d41055d0ac5859a69a8b75d9a201cd226999f4f00cc983f13

RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y \
        dotnet-sdk-6.0 \
        dotnet-sdk-7.0 \
        cmake \
        clang \
        make

WORKDIR /project
