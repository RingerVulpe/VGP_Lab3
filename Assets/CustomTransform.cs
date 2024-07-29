using UnityEngine;

public class CustomTransform : MonoBehaviour
{
    private Vector3 position;
    private Quaternion rotation;
    private Vector3 scale;

    void Start()
    {
        // Initialize the custom transform properties
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.localScale;
    }

    void Update()
    {
        // Input for translation
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 5.0f;
        float moveY = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f;
        position += new Vector3(moveX, 0, moveY);

        // Input for rotation
        if (Input.GetKey(KeyCode.Q))
        {
            rotation = MultiplyQuaternions(rotation, Quaternion.Euler(0, -90 * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotation = MultiplyQuaternions(rotation, Quaternion.Euler(0, 90 * Time.deltaTime, 0));
        }

        // Input for scaling
        if (Input.GetKey(KeyCode.Z))
        {
            scale += Vector3.one * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.X))
        {
            scale -= Vector3.one * Time.deltaTime;
        }

        // Update the object's transform
        ApplyCustomTransform();
    }

    void ApplyCustomTransform()
    {
        // Create the transformation matrix manually in the correct order
        Matrix4x4 scaleMatrix = CreateScaleMatrix(scale);
        Matrix4x4 rotationMatrix = CreateRotationMatrix(rotation);
        Matrix4x4 translationMatrix = CreateTranslationMatrix(position);

        // Combine the transformations in the correct order: scale -> rotation -> translation
        Matrix4x4 transformationMatrix = MultiplyMatrices(translationMatrix, MultiplyMatrices(rotationMatrix, scaleMatrix));

        // Apply the transformation to the object
        transform.position = new Vector3(transformationMatrix.m03, transformationMatrix.m13, transformationMatrix.m23);
        transform.rotation = rotation;
        transform.localScale = new Vector3(transformationMatrix.GetColumn(0).magnitude, transformationMatrix.GetColumn(1).magnitude, transformationMatrix.GetColumn(2).magnitude);
    }

    Matrix4x4 CreateTranslationMatrix(Vector3 pos)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m03 = pos.x;
        matrix.m13 = pos.y;
        matrix.m23 = pos.z;
        return matrix;
    }

    Matrix4x4 CreateRotationMatrix(Quaternion q)
    {
        float xx = q.x * q.x;
        float yy = q.y * q.y;
        float zz = q.z * q.z;
        float xy = q.x * q.y;
        float xz = q.x * q.z;
        float yz = q.y * q.z;
        float wx = q.w * q.x;
        float wy = q.w * q.y;
        float wz = q.w * q.z;

        Matrix4x4 matrix = new Matrix4x4();
        matrix.m00 = 1 - 2 * (yy + zz);
        matrix.m01 = 2 * (xy - wz);
        matrix.m02 = 2 * (xz + wy);
        matrix.m03 = 0;

        matrix.m10 = 2 * (xy + wz);
        matrix.m11 = 1 - 2 * (xx + zz);
        matrix.m12 = 2 * (yz - wx);
        matrix.m13 = 0;

        matrix.m20 = 2 * (xz - wy);
        matrix.m21 = 2 * (yz + wx);
        matrix.m22 = 1 - 2 * (xx + yy);
        matrix.m23 = 0;

        matrix.m30 = 0;
        matrix.m31 = 0;
        matrix.m32 = 0;
        matrix.m33 = 1;

        return matrix;
    }

    Matrix4x4 CreateScaleMatrix(Vector3 s)
    {
        Matrix4x4 matrix = Matrix4x4.identity;
        matrix.m00 = s.x;
        matrix.m11 = s.y;
        matrix.m22 = s.z;
        return matrix;
    }

    Matrix4x4 MultiplyMatrices(Matrix4x4 a, Matrix4x4 b)
    {
        Matrix4x4 result = new Matrix4x4();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                result[i, j] = a[i, 0] * b[0, j] + a[i, 1] * b[1, j] + a[i, 2] * b[2, j] + a[i, 3] * b[3, j];
            }
        }
        return result;
    }

    Quaternion MultiplyQuaternions(Quaternion q1, Quaternion q2)
    {
        return new Quaternion(
            q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y,
            q1.w * q2.y + q1.y * q2.w + q1.z * q2.x - q1.x * q2.z,
            q1.w * q2.z + q1.z * q2.w + q1.x * q2.y - q1.y * q2.x,
            q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z
        );
    }
}
