using System;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Opengl;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Java.Lang;
using Java.Nio;
using Javax.Microedition.Khronos.Opengles;

namespace ThumbnailPicker.TestCases
{
    public class VideoSurfaceViewTest : IImageTest
    {
        VideoRender videoRenderer;
        MediaPlayer mediaPlayer = null;


        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void Load(Context context, Android.Net.Uri source, int videoWidth, int videoHeight)
        {
            /*
            SetEGLContextClientVersion(2);
            mediaPlayer = new MediaPlayer();
            videoRenderer = new VideoRender(context);
            videoRenderer.setMediaPlayer(mediaPlayer);

            SetRenderer(videoRenderer);
            */
        }

        public Bitmap? RunTest(long currentTargetMs)
        {
            throw new NotImplementedException();
        }


        //public override void OnResume()
        //{
            /*
            queueEvent(new Runnable()
            {
                public void run()
                {
                    mRenderer.setMediaPlayer(mMediaPlayer);
                }
            });
            */
            //videoRenderer.setMediaPlayer(mediaPlayer);
            //base.OnResume();
        //}

        /*
        internal Bitmap GetBitmap()
        {
            return videoRenderer.GetBitmap();
        }

        */

        internal class VideoRender : Java.Lang.Object, GLSurfaceView.IRenderer, SurfaceTexture.IOnFrameAvailableListener
        {
            private string TAG = "VideoRender";

            private const int FLOAT_SIZE_BYTES = 4;
            private const int TRIANGLE_VERTICES_DATA_STRIDE_BYTES = 5 * FLOAT_SIZE_BYTES;
            private const int TRIANGLE_VERTICES_DATA_POS_OFFSET = 0;
            private const int TRIANGLE_VERTICES_DATA_UV_OFFSET = 3;
            private float[] mTriangleVerticesData = new float[] {
                // X, Y, Z, U, V
                -1.0f,  -1.0f, 0, 0f, 0f,
                 1.0f,  -1.0f, 0, 1f, 0f,
                -1.0f,   1.0f, 0, 0f, 1f,
                 1.0f,   1.0f, 0, 1f, 1f,
            };

            private FloatBuffer mTriangleVertices;

            private object _frameSyncObject = new object(); // guards mFrameAvailable



            private float[] mMVPMatrix = new float[16];
            private float[] mSTMatrix = new float[16];

            private int mProgram;
            private int mTextureID;
            private int muMVPMatrixHandle;
            private int muSTMatrixHandle;
            private int maPositionHandle;
            private int maTextureHandle;

            private SurfaceTexture surfaceTexture;
            private bool updateSurface = false;

            private static int GL_TEXTURE_EXTERNAL_OES = 0x8D65;

            private MediaPlayer mediaPlayer;

            public VideoRender(Context context)
            {
                mTriangleVertices = ByteBuffer.AllocateDirect(
                    mTriangleVerticesData.Length * FLOAT_SIZE_BYTES)
                        .Order(ByteOrder.NativeOrder()).AsFloatBuffer();
                mTriangleVertices.Put(mTriangleVerticesData).Position(0);

                Android.Opengl.Matrix.SetIdentityM(mSTMatrix, 0);
            }

            public void setMediaPlayer(MediaPlayer player)
            {
                mediaPlayer = player;
            }

            public void OnDrawFrame(IGL10 glUnused)
            {

                lock (_frameSyncObject)
                {
                    if (updateSurface)
                    {
                        surfaceTexture.UpdateTexImage();
                        surfaceTexture.GetTransformMatrix(mSTMatrix);
                        updateSurface = false;
                    }
                }

                GLES20.GlClearColor(0.0f, 1.0f, 0.0f, 1.0f);
                GLES20.GlClear(GLES20.GlDepthBufferBit | GLES20.GlColorBufferBit);

                GLES20.GlUseProgram(mProgram);
                checkGlError("glUseProgram");

                GLES20.GlActiveTexture(GLES20.GlTexture0);
                GLES20.GlBindTexture(GL_TEXTURE_EXTERNAL_OES, mTextureID);

                mTriangleVertices.Position(TRIANGLE_VERTICES_DATA_POS_OFFSET);
                GLES20.GlVertexAttribPointer(maPositionHandle, 3, GLES20.GlFloat, false,
                    TRIANGLE_VERTICES_DATA_STRIDE_BYTES, mTriangleVertices);
                checkGlError("glVertexAttribPointer maPosition");
                GLES20.GlEnableVertexAttribArray(maPositionHandle);
                checkGlError("glEnableVertexAttribArray maPositionHandle");

                mTriangleVertices.Position(TRIANGLE_VERTICES_DATA_UV_OFFSET);
                GLES20.GlVertexAttribPointer(maTextureHandle, 3, GLES20.GlFloat, false,
                    TRIANGLE_VERTICES_DATA_STRIDE_BYTES, mTriangleVertices);
                checkGlError("glVertexAttribPointer maTextureHandle");
                GLES20.GlEnableVertexAttribArray(maTextureHandle);
                checkGlError("glEnableVertexAttribArray maTextureHandle");

                Android.Opengl.Matrix.SetIdentityM(mMVPMatrix, 0);
                GLES20.GlUniformMatrix4fv(muMVPMatrixHandle, 1, false, mMVPMatrix, 0);
                GLES20.GlUniformMatrix4fv(muSTMatrixHandle, 1, false, mSTMatrix, 0);

                GLES20.GlDrawArrays(GLES20.GlTriangleStrip, 0, 4);
                checkGlError("glDrawArrays");
                GLES20.GlFinish();

            }


            public void OnSurfaceChanged(IGL10 glUnused, int width, int height)
            {
                //System.Diagnostics.Debugger.Break();

            }


            //public void onSurfaceCreated(GL10 glUnused, EGLConfig config)

            public void OnSurfaceCreated(IGL10 glUnused, Javax.Microedition.Khronos.Egl.EGLConfig config)
            {
                mProgram = createProgram();
                if (mProgram == 0)
                {
                    return;
                }
                maPositionHandle = GLES20.GlGetAttribLocation(mProgram, "aPosition");
                checkGlError("glGetAttribLocation aPosition");
                if (maPositionHandle == -1)
                {
                    throw new RuntimeException("Could not get attrib location for aPosition");
                }
                maTextureHandle = GLES20.GlGetAttribLocation(mProgram, "aTextureCoord");
                checkGlError("glGetAttribLocation aTextureCoord");
                if (maTextureHandle == -1)
                {
                    throw new RuntimeException("Could not get attrib location for aTextureCoord");
                }

                muMVPMatrixHandle = GLES20.GlGetUniformLocation(mProgram, "uMVPMatrix");
                checkGlError("glGetUniformLocation uMVPMatrix");
                if (muMVPMatrixHandle == -1)
                {
                    throw new RuntimeException("Could not get attrib location for uMVPMatrix");
                }

                muSTMatrixHandle = GLES20.GlGetUniformLocation(mProgram, "uSTMatrix");
                checkGlError("glGetUniformLocation uSTMatrix");
                if (muSTMatrixHandle == -1)
                {
                    throw new RuntimeException("Could not get attrib location for uSTMatrix");
                }


                int[] textures = new int[1];
                GLES20.GlGenTextures(1, textures, 0);

                mTextureID = textures[0];
                GLES20.GlBindTexture(GL_TEXTURE_EXTERNAL_OES, mTextureID);
                checkGlError("glBindTexture mTextureID");

                GLES20.GlTexParameterf(GL_TEXTURE_EXTERNAL_OES, GLES20.GlTextureMinFilter, GLES20.GlNearest);
                GLES20.GlTexParameterf(GL_TEXTURE_EXTERNAL_OES, GLES20.GlTextureMagFilter, GLES20.GlLinear);

                
                // Create the SurfaceTexture that will feed this textureID,
                // and pass it to the MediaPlayer
                surfaceTexture = new SurfaceTexture(mTextureID);
                surfaceTexture.SetOnFrameAvailableListener(this);

                Surface surface = new Surface(surfaceTexture);
                mediaPlayer.SetSurface(surface);
                mediaPlayer.SetScreenOnWhilePlaying(true);
                surface.Release();

                try
                {
                    mediaPlayer.Prepare();
                }
                catch (Java.IO.IOException)
                {
                    Log.Error(TAG, "media player prepare failed");
                }


                lock (_frameSyncObject)
                {
                    updateSurface = false;
                }

                mediaPlayer.Start();
            }


            public void OnFrameAvailable(SurfaceTexture surfaceTexture)
            {
                lock (_frameSyncObject)
                {
                    updateSurface = true;
                }
            }


            int loadShader(int shaderType, string source)
            {
                int shader = GLES20.GlCreateShader(shaderType);
                if (shader != 0)
                {
                    GLES20.GlShaderSource(shader, source);
                    GLES20.GlCompileShader(shader);
                    int[] compiled = new int[1];
                    GLES20.GlGetShaderiv(shader, GLES20.GlCompileStatus, compiled, 0);
                    if (compiled[0] == 0)
                    {
                        Log.Error(TAG, "Could not compile shader " + shaderType + ":");
                        Log.Error(TAG, GLES20.GlGetShaderInfoLog(shader));
                        GLES20.GlDeleteShader(shader);
                        shader = 0;
                    }
                }
                return shader;
            }

            private int createProgram()
            {


                var mVertexShader = @"
                uniform mat4 uMVPMatrix;
                uniform mat4 uSTMatrix;
                attribute vec4 aPosition;
                attribute vec4 aTextureCoord;
                varying vec2 vTextureCoord;
                void main() {
                    gl_Position = uMVPMatrix * aPosition;
                    vTextureCoord = (uSTMatrix * aTextureCoord).xy;
                }
            ";


                var mFragmentShader = @"
                    #extension GL_OES_EGL_image_external : require
                    precision mediump float;
                    varying vec2 vTextureCoord;
                    uniform samplerExternalOES sTexture;
                    void main() {
                      gl_FragColor = texture2D(sTexture, vTextureCoord);
                    }
             ";

                int vertexShader = loadShader(GLES20.GlVertexShader, mVertexShader);
                if (vertexShader == 0)
                {
                    return 0;
                }
                int pixelShader = loadShader(GLES20.GlFragmentShader, mFragmentShader);
                if (pixelShader == 0)
                {
                    return 0;
                }

                int program = GLES20.GlCreateProgram();
                if (program != 0)
                {
                    GLES20.GlAttachShader(program, vertexShader);
                    checkGlError("glAttachShader");
                    GLES20.GlAttachShader(program, pixelShader);
                    checkGlError("glAttachShader");
                    GLES20.GlLinkProgram(program);
                    int[] linkStatus = new int[1];
                    GLES20.GlGetProgramiv(program, GLES20.GlLinkStatus, linkStatus, 0);
                    if (linkStatus[0] != GLES20.GlTrue)
                    {
                        Log.Error(TAG, "Could not link program: ");
                        Log.Error(TAG, GLES20.GlGetProgramInfoLog(program));
                        GLES20.GlDeleteProgram(program);
                        program = 0;
                    }
                }
                return program;
            }

            private void checkGlError(string op)
            {
                int error;
                while ((error = GLES20.GlGetError()) != GLES20.GlNoError)
                {
                    Log.Error(TAG, op + ": glError " + error);
                    throw new RuntimeException(op + ": glError " + error);
                }
            }

            internal Bitmap GetBitmap()
            {
                var bitmap = Bitmap.CreateBitmap(mediaPlayer.VideoWidth, mediaPlayer.VideoHeight, Bitmap.Config.Argb8888);
                // mSurface.

                //surfaceTexture.

                return bitmap;
            }
        }
    }
}

