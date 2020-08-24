using System;
using System.Collections.Generic;
using System.Text;

namespace aria
{
    static class Message
    {
       
     public const string MSG_DOWNLOAD_COMPLETED = "CUID#%d - The download for one segment completed successfully.";
     public const string MSG_NO_SEGMENT_AVAILABLE= "CUID#%d - No segment available.";
     public const string MSG_CONNECTING_TO_SERVER= "CUID#%d - Connecting to %s:%d";
     public const string MSG_SEGMENT_CHANGED = "CUID#%d - The segment changed. We send the request again with new Range header.";
     public const string MSG_REDIRECT= "CUID#%d - Redirecting to %s";
     public const string MSG_SENDING_HTTP_REQUEST= "CUID#%d - Sending the request:\n%s";
     public const string MSG_RECEIVE_RESPONSE= "CUID#%d - Response received:\n%s";
     public const string MSG_DOWNLOAD_ABORTED= "CUID#%d - Download aborted.";
     public const string MSG_RESTARTING_DOWNLOAD= "CUID#%d - Restarting the download.";
     public const string MSG_MAX_RETRY = "CUID#%d - The retry count reached its max value. Download aborted.";
     
     public const string MSG_SEGMENT_FILE_EXISTS = "The segment file %s exists.";
     public const string MSG_SEGMENT_FILE_DOES_NOT_EXIST= "The segment file %s does not exist.";
     public const string MSG_SAVING_SEGMENT_FILE= "Saving the segment file %s";
     public const string MSG_SAVED_SEGMENT_FILE= "The segment file was saved successfully.";
     public const string MSG_LOADING_SEGMENT_FILE= "Loading the segment file %s.";
     public const string MSG_LOADED_SEGMENT_FILE= "The segment file was loaded successfully.";
     
     public const string EX_TIME_OUT= "Timeout.";
     public const string EX_INVALID_CHUNK_SIZE= "Invalid chunk size.";
     public const string EX_TOO_LARGE_CHUNK= "Too large chunk. size = %d";
     public const string EX_INVALID_HEADER= "Invalid header.";
     public const string EX_NO_HEADER= "No header found.";
     public const string EX_NO_STATUS_HEADER= "No status header.";
     public const string EX_PROXY_CONNECTION_FAILED= "Proxy connection failed.";
     public const string EX_FILENAME_MISMATCH= "The requested filename and the previously registered one are not same. %s != %s";
     public const string EX_BAD_STATUS= "The response status is not successful. status = %d";
     public const string EX_TOO_LARGE_FILE= "Too large file size. size = %d";
     public const string EX_TRANSFER_ENCODING_NOT_SUPPORTED= "Transfer encoding %s is not supported.";
    }
}
