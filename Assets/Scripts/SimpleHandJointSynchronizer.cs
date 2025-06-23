using UnityEngine;

/// <summary>
/// 一个简单的数据容器脚本。
/// 它的唯一作用是持有一个对本地追踪关节（m_HandJoint）的引用，
/// 以便让 NetworkHandJoint 脚本能够找到并追踪它。
/// </summary>
public class SimpleHandJointSynchronizer : MonoBehaviour
{
    // 这个 Transform 将由 LocalHandDataSourceManager 脚本在运行时设置。
    public Transform m_HandJoint;
}