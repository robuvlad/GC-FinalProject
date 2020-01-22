using OpenGL;
using System;
using Tao.FreeGlut;

namespace FinalProject
{
    class Program
    {
        private static int width = 1200, height = 720;
        private static ShaderProgram program;

        private static VBO<Vector3> cube;
        private static VBO<Vector3> cubeColor;
        private static VBO<Vector2> cubeUV;
        private static VBO<Vector3> cubeNormals;
        private static VBO<int> cubeElements;

        //rotation
        private static float xangle, yangle;
        private static float deltaTime = 0.002f;

        //texture
        private static Texture crateTexture;

        //enable lighting
        private static bool lighting = true, autoRotate = true, fullScreen = false;

        static void Main(string[] args)
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("Project");

            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
            Glut.glutKeyboardFunc(OnKeyboardDown);
            Glut.glutKeyboardFunc(OnKeyboardUp);
            Glut.glutCloseFunc(OnClose);
            Glut.glutReshapeFunc(OnReshape);

            Gl.Enable(EnableCap.DepthTest);

            //program = new ShaderProgram(VertexShader, FragmentShader);
            program = new ShaderProgram(VertexShaderTexture, FragmentShaderTexture);

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

            program["light_direction"].SetValue(new Vector3(0, 1, 1));
            //program["light_direction2"].SetValue(new Vector3(0, 0, 1));
            program["enable_lighting"].SetValue(lighting);

            //load texture
            crateTexture = new Texture("texture2.jpg");

            //create cube vertices
            cube = new VBO<Vector3>(new Vector3[] {
                new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(1, 1, 1), //top
                new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1), new Vector3(1, -1, -1), //bottom
                new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1), //front
                new Vector3(1, -1, -1), new Vector3(-1, -1, -1), new Vector3(-1, 1, -1), new Vector3(1, 1, -1), //back
                new Vector3(-1, 1, 1), new Vector3(-1, 1, -1), new Vector3(-1, -1, -1), new Vector3(-1, -1, 1), //left
                new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1) //right
            });
            //cube normals
            cubeNormals = new VBO<Vector3>(new Vector3[] {
                new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 0),
                new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0), new Vector3(0, -1, 0),
                new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1),
                new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1), new Vector3(0, 0, -1),
                new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0), new Vector3(-1, 0, 0),
                new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 0)
            });
            //texture for cube
            cubeUV = new VBO<Vector2>(new Vector2[] {
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
                new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
            });

            cubeElements = new VBO<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                   21, 22, 23 }, BufferTarget.ElementArrayBuffer);

            Glut.glutMainLoop();
        }

        private static void OnDisplay()
        {

        }

        private static void OnClose()
        {
            cube.Dispose();
            cubeNormals.Dispose();
            cubeUV.Dispose();
            cubeElements.Dispose();

            crateTexture.Dispose();
            program.DisposeChildren = true;
            program.Dispose();
        }

        private static void OnRenderFrame()
        {
            if (autoRotate)
            {
                xangle += deltaTime;
                yangle += deltaTime / 2;
            }


            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Gl.UseProgram(program);
            Gl.BindTexture(crateTexture); //bind texture

            //enable / disable lighting
            program["enable_lighting"].SetValue(lighting);

            program["model_matrix"].SetValue(Matrix4.CreateRotationX(yangle) * Matrix4.CreateRotationY(xangle)
                * Matrix4.CreateTranslation(new Vector3(-1.6f, 0, 0)));

            Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeNormals, program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV"); //set texture
            Gl.BindBuffer(cubeElements);

            Gl.DrawElements(BeginMode.Quads, cubeElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            //draw the square
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(yangle) * Matrix4.CreateRotationX(xangle) 
                * Matrix4.CreateTranslation(new Vector3(1.6f, 0, 0)));

            Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeNormals, program, "vertexNormal");
            Gl.BindBufferToShaderAttribute(cubeUV, program, "vertexUV"); //set texture
            Gl.BindBuffer(cubeElements);
            //Gl.BindBufferToShaderAttribute(cubeColor, program, "vertexColor"); //set color

            Gl.DrawElements(BeginMode.Quads, cubeElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);


            Glut.glutSwapBuffers();
        }

        private static void OnReshape(int width, int height)
        {
            Program.width = width;
            Program.height = height;

            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
        }

        private static void OnKeyboardDown(byte key, int x, int y)
        {
            if (key == 'q') Glut.glutLeaveMainLoop();
        }

        private static void OnKeyboardUp(byte key, int x, int y)
        {
            if (key == 'l')
                lighting = !lighting;
            else if (key == ' ')
                autoRotate = !autoRotate;
            else if (key == 'f')
            {
                fullScreen = !fullScreen;
                if (fullScreen) Glut.glutFullScreen();
                else
                {
                    Glut.glutPositionWindow(0, 0);
                    Glut.glutReshapeWindow(1280, 720);
                }
            }
        }

        public static string VertexShaderTexture = @"
#version 130

in vec3 vertexPosition;
in vec3 vertexNormal;
in vec2 vertexUV;

out vec3 normal;
out vec2 uv;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;

void main(void) {
    normal = normalize((model_matrix * vec4(vertexNormal, 0)).xyz);
    uv = vertexUV;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";


        public static string FragmentShaderTexture = @"
#version 130

uniform vec3 light_direction;
uniform vec3 light_direction2;
uniform sampler2D texture;
uniform bool enable_lighting;

in vec3 normal;
in vec2 uv;

out vec4 fragment;

void main(void) {
    float diffuse1 = max(dot(normal, light_direction), 0);
    float ambient1 = 0.4f;
    float lighting1 = (enable_lighting ? max(diffuse1, ambient1) : 1.0f);

    float diffuse2 = max(dot(normal, light_direction2), 0);
    float ambient2 = 0.3f;
    float lighting2 = (enable_lighting ? max(diffuse2, ambient2) : 1.0f);

    float specular = 0.2f;
    
    vec4 sample = texture2D(texture, uv);
    fragment = vec4(sample.xyz * lighting1 * lighting2, sample.a);
}
";
    }
}
