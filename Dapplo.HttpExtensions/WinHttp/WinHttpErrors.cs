namespace Dapplo.HttpExtensions.WinHttp
{
	/// <summary>
	/// The possible errors which 
	/// </summary>
	public enum WinHttpErrors
	{
		Success = 0,
		OutOfHandles = 12001,                       // ERROR_WINHTTP_OUT_OF_HANDLES 
		Timeout = 12002,                            // ERROR_WINHTTP_TIMEOUT
		InternalError = 12004,                      // ERROR_WINHTTP_INTERNAL_ERROR 
		InvalidUrl = 12005,                         // ERROR_WINHTTP_INVALID_URL
		UnrecognizedScheme = 12006,                 // ERROR_WINHTTP_UNRECOGNIZED_SCHEME
		NameNotResolved = 12007,                    // ERROR_WINHTTP_NAME_NOT_RESOLVED
		InvalidOption = 12009,                      // ERROR_WINHTTP_INVALID_OPTION 
		OptionNotSettable = 12011,                  // ERROR_WINHTTP_OPTION_NOT_SETTABLE
		Shutdown = 12012,                           // ERROR_WINHTTP_SHUTDOWN 

		LoginFailure = 12015,                       // ERROR_WINHTTP_LOGIN_FAILURE
		OperationCancelled = 12017,                 // ERROR_WINHTTP_OPERATION_CANCELLED 
		IncorrectHandleType = 12018,                // ERROR_WINHTTP_INCORRECT_HANDLE_TYPE
		IncorrectHandleState = 12019,               // ERROR_WINHTTP_INCORRECT_HANDLE_STATE
		CannotConnect = 12029,                      // ERROR_WINHTTP_CANNOT_CONNECT
		ConnectionError = 12030,                    // ERROR_WINHTTP_CONNECTION_ERROR 
		ResendRequest = 12032,                      // ERROR_WINHTTP_RESEND_REQUEST

		AuthCertNeeded = 12044,                     // ERROR_WINHTTP_CLIENT_AUTH_CERT_NEEDED 

		CannotCallBeforeOpen = 12100,               // ERROR_WINHTTP_CANNOT_CALL_BEFORE_OPEN 
		CannotCallBeforeSend = 12101,               // ERROR_WINHTTP_CANNOT_CALL_BEFORE_SEND
		CannotCallAfterSend = 12102,                // ERROR_WINHTTP_CANNOT_CALL_AFTER_SEND
		CannotCallAfterOpen = 12103,                // ERROR_WINHTTP_CANNOT_CALL_AFTER_OPEN

		HeaderNotFound = 12150,                     // ERROR_WINHTTP_HEADER_NOT_FOUND
		InvalidServerResponse = 12152,              // ERROR_WINHTTP_INVALID_SERVER_RESPONSE 
		InvalidHeader = 12153,                      // ERROR_WINHTTP_INVALID_HEADER 
		InvalidQueryRequest = 12154,                // ERROR_WINHTTP_INVALID_QUERY_REQUEST
		HeaderAlreadyExists = 12155,                // ERROR_WINHTTP_HEADER_ALREADY_EXISTS 
		RedirectFailed = 12156,                     // ERROR_WINHTTP_REDIRECT_FAILED

		AutoProxyServiceError = 12178,              // ERROR_WINHTTP_AUTO_PROXY_SERVICE_ERROR
		BadAutoProxyScript = 12166,                 // ERROR_WINHTTP_BAD_AUTO_PROXY_SCRIPT 
		UnableToDownloadScript = 12167,             // ERROR_WINHTTP_UNABLE_TO_DOWNLOAD_SCRIPT

		NotInitialized = 12172,                     // ERROR_WINHTTP_NOT_INITIALIZED 
		SecureFailure = 12175,                      // ERROR_WINHTTP_SECURE_FAILURE

		SecureCertDateInvalid = 12037,              // ERROR_WINHTTP_SECURE_CERT_DATE_INVALID
		SecureCertCnInvalid = 12038,                // ERROR_WINHTTP_SECURE_CERT_CN_INVALID
		SecureInvalidCa = 12045,                    // ERROR_WINHTTP_SECURE_INVALID_CA
		SecureCertRevFailed = 12057,                // ERROR_WINHTTP_SECURE_CERT_REV_FAILED 
		SecureChannelError = 12157,                 // ERROR_WINHTTP_SECURE_CHANNEL_ERROR
		SecureInvalidCert = 12169,                  // ERROR_WINHTTP_SECURE_INVALID_CERT 
		SecureCertRevoked = 12170,                  // ERROR_WINHTTP_SECURE_CERT_REVOKED 
		SecureCertWrongUsage = 12179,               // ERROR_WINHTTP_SECURE_CERT_WRONG_USAGE

		AudodetectionFailed = 12180,                // ERROR_WINHTTP_AUTODETECTION_FAILED
		HeaderCountExceeded = 12181,                // ERROR_WINHTTP_HEADER_COUNT_EXCEEDED
		HeaderSizeOverflow = 12182,                 // ERROR_WINHTTP_HEADER_SIZE_OVERFLOW
		ChunkedEncodingHeaderSizeOverflow = 12183,  // ERROR_WINHTTP_CHUNKED_ENCODING_HEADER_SIZE_OVERFLOW 
		ResponseDrainOverflow = 12184,              // ERROR_WINHTTP_RESPONSE_DRAIN_OVERFLOW
		ClientCertNoPrivateKey = 12185,             // ERROR_WINHTTP_CLIENT_CERT_NO_PRIVATE_KEY 
		ClientCertNoAccessPrivateKey = 12186,       // ERROR_WINHTTP_CLIENT_CERT_NO_ACCESS_PRIVATE_KEY 
	}
}
