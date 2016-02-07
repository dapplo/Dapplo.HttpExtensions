namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	/// Marker for the response
	/// </summary>
	public enum HttpParts
	{
		/// <summary>
		/// Marks the class as a request class
		/// </summary>
		Request,
		/// <summary>
		/// Specifies that the class is a multi-part container
		/// </summary>
		MultiPart,
		/// <summary>
		/// The property specifies the boundary of a multi-part
		/// </summary>
		MultipartBoundary,
		/// <summary>
		/// The property specifies the name of the content in a multi-part post
		/// </summary>
		MultipartName,
		/// <summary>
		/// The property specifies the filename of the content in a multi-part post
		/// </summary>
		MultipartFilename,
		/// <summary>
		/// Specifies the content for uploading
		/// </summary>
		RequestContent,
		/// <summary>
		/// Specifies the content-type for uploading
		/// </summary>
		RequestContentType,
		/// <summary>
		/// Specifies the request headers
		/// </summary>
		RequestHeaders,

		/// <summary>
		/// Marks the class as a reponse class
		/// </summary>
		Response,
		/// <summary>
		/// The property will get the response content, HttpResponseMessage can also be used
		/// </summary>
		ResponseContent,
		/// <summary>
		/// The property will get the response content, when an error occured
		/// </summary>
		ResponseErrorContent,
		/// <summary>
		/// Specifies the content-type, either for uploading or for the response
		/// </summary>
		ResponseContentType,
		/// <summary>
		/// The Http-Status code, should be of type HttpStatusCode
		/// </summary>
		ResponseStatuscode,
		/// <summary>
		/// Marks HttpResponseHeaders, 
		/// </summary>
		ResponseHeaders,
	}
}
