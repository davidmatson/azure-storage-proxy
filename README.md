Azure Storage Proxy
===
Azure Storage Proxy is a local HTTP proxy for Azure Storage that automatically signs requests using local credentials.

One of the hardest parts of experimenting with the Azure Storage REST API is adding the required Authorization header,
which must be computed separately for each request. This local proxy automatically computes the Authorization header
using credentials provided in App.config, enabling the use of standard HTTP tools such as Fiddler and curl to experiment
with the Azure Storage REST API.
