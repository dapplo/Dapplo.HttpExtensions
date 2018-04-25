# Posting content

The library tries to take a lot of work away from the developer, and tries to use sane defaults for it's behaviour.

You can just post a System.Drawing.Bitmap, and the library will generate a binary upload with the mimetype being "imgage/png"
The configuration for this can be modified by using the BitmapConfiguration and settings this on the used HttpBehaviour.
For instance, if you like the default to be gif, use the following line:
```HttpBehaviour.Current.SetConfig(new BitmapConfiguration {Format = ImageFormat.Gif});```
There are converters for BitmapSource, XDocument, SyndicationFeed, Stream, byte[], or a IEnumerable<KeyValuePair<string, string>> (e.g. IDictionary<string, string>)

Also posting JSON is quite easy, but you will need to have a serializer.
By adding Dapplo.HttpExtensions.JsonNet you can configure this with one line:
```HttpExtensionsGlobals.JsonSerializer = new JsonNetJsonSerializer();```
Or
```JsonNetJsonSerializer.RegisterGlobally();```

Now if you would do something like this:

```
var exampleUri = new Uri("https://myserver/api/create");
var someObject = ....

exampleUri.PostAsync<string>(someObject);
```

In this case, the library tries to serialize someObject by every means possible, and will when you did not add other "converters" use JsonNetJsonSerializer.
In this case it uses Json.NET to serialize the object, and will create a "string" content with json and use the mimetype "application/json".

To have more control over the content which is posted, you can supply a self created HttpContent object.
There is a factory to do the heavy lifting, allowing more controll, here an example which specifies a different mimetype:
var content = HttpContentFactory.Create(someObject);
content.SetContentType("application/json-patch+json");

Sending is than easy:
```exampleUri.PostAsync<string>(content);```

You can also "post" an implementation of a custom class, where you can use your own properties.
Here is an example to generate a multipart request with a json and a png:

```
    [HttpRequest]
    public class MyMultiPartRequest<TBitmap>
    {
        [HttpPart(HttpParts.RequestMultipartName, Order = 1)]
        public string BitmapContentName { get; set; } = "File";

        [HttpPart(HttpParts.RequestContentType, Order = 1)]
        public string BitmapContentType { get; set; } = "image/png";

        [HttpPart(HttpParts.RequestMultipartFilename, Order = 1)]
        public string BitmapFileName { get; set; } = "empty.png";

        [HttpPart(HttpParts.RequestContentType, Order = 0)]
        public string ContentType { get; set; } = "application/json";

        [HttpPart(HttpParts.RequestHeaders)]
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

        [HttpPart(HttpParts.RequestContent, Order = 0)]
        public object JsonInformation { get; set; }

        [HttpPart(HttpParts.RequestContent, Order = 1)]
        public TBitmap OurBitmap { get; set; }
    }
```