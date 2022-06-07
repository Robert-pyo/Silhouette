using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPool<T> : IObjectPool<T>
{
    public Queue<T> pool = new Queue<T>();
    
    public delegate T CreateFunc();
    public delegate void Action(T elem);
    
    private readonly CreateFunc createFunc;
    private readonly Action actionOnGet;
    private readonly Action actionOnRelease;
    private readonly Action actionOnDestroy;

    public int CountActive { get; private set; }
    public int CountAll { get; private set; }
    public int CountInactive { get; set; }
    
    // 1. 최대 사이즈(maxSize)를 가지고 있음
    public uint maxSize;

    public ObjectPool(CreateFunc createFunc, Action actionOnGet, Action actionOnRelease, Action actionOnDestroy, uint maxSize)
    {
        this.createFunc = createFunc;
        this.actionOnGet = actionOnGet;
        this.actionOnRelease = actionOnRelease;
        this.actionOnDestroy = actionOnDestroy;
        this.maxSize = maxSize;
    }
    
    // 2. Get()함수를 통해 오브젝트를 가져옴
    // ㄴ Get()함수는 오브젝트 풀에 오브젝트가 있다면 그걸 SetActive(true)를 하여 건네주고
    public T Get()
    {
        if (pool.Count == 0)
        {
            CountActive++;
            CountAll = CountActive + CountInactive;
            return createFunc.Invoke();
        }

        var elem = pool.Dequeue();
        actionOnGet.Invoke(elem);
        
        CountActive++;
        CountInactive--;
        CountAll = CountActive + CountInactive;
        
        return elem;
    }
    
    // ㄴ 없다면 새로 생성하여 건네줌

    // 3. 씬에 생성된 오브젝트 들은 사용이 끝났다면 Release() 함수를 통해 오브젝트 풀에 되돌려(저장해)줄 수 있음
    // ㄴ 이때 저장되는 오브젝트의 개수는 maxSize를 넘길 수 없음
    // ㄴ 이는 지정된 개수 이상으로 갑자기 많은 개체가 생성되었을때 최대 개수만큼은 저장하고 나머지는 파괴해주어
    // ㄴ 필요이상으로 오브젝트 풀에 저장하지 않도록 하기 위함
    public void Release(T element)
    {
        if (pool.Count < maxSize)
        {
            pool.Enqueue(element);
            actionOnRelease(element);
            CountInactive++;
            CountActive--;
        }
        else
        {
            actionOnDestroy(element);
            CountActive--;
        }
        
        CountAll = CountActive + CountInactive;
    }

    public void Clear()
    {
        foreach (var t in pool)
        {
            actionOnDestroy.Invoke(t);
        }
        
        pool.Clear();
        
        CountActive = 0;
        CountInactive = 0;
        CountAll = 0;
    }
}

public interface IObjectPool<T>
{
    int CountInactive { get; set; }
    
    T Get();
    void Release(T element);
    void Clear();
}
