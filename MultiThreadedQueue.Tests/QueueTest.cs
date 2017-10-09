using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;

namespace MultiThreadedQueue.Tests
{
    [TestFixture]
    public class QueueTest
    {
        [Test]
        public async Task Queue_ShouldWaitWhenEmpty_Sucess()
        {
            //Arrange
            var queue = new MultiThreadedQueue<int>();
            var readTask = Task.Run(() =>
            {
                queue.Pop();
            });
            //Act
            var t = await Task.WhenAny(readTask, Task.Delay(3000));
            //Assert - write should complete first
            Assert.That(t.Id, Is.Not.EqualTo(readTask.Id));
        }

        [Test]
        public async Task Queue_ShouldWaitWhenEmpty_WaitTillPush()
        {
            //Arrange
            var queue = new MultiThreadedQueue<int>();
            var readTask = Task.Run(() =>
            {
                queue.Pop();
            });
            var writeTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                queue.Push(1);
            });
            //Act
            var t = await Task.WhenAny(readTask, writeTask);
            //Assert - write should complete first
            Assert.That(t.Id, Is.EqualTo(writeTask.Id));
        }

        [Test]
        public void Queue_ShouldThrowException_QueueAtCapacity()
        {
            //Arrange
            var maxItems = 3;
            var queue = new MultiThreadedQueue<int>(maxItems);
            //Act
            var writeTasks = new List<Task>();
            for (int i = 0; i < maxItems + 1; i++)
            {
                writeTasks.Add(Task.Run(() => { queue.Push(1); }));
            }
            //Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => { await Task.WhenAll(writeTasks); });
        }

        [Test]
        public void Queue_ShouldPopSingle_Success()
        {
            //Arrange
            var item = new Random().Next();
            var queue = new MultiThreadedQueue<int>();
            //Act
            queue.Push(item);
            var actual = queue.Pop();
            //Assert
            Assert.That(actual, Is.EqualTo(item));
        }

        [Test]
        public void Queue_ShouldPopMultiple_Success()
        {
            //Arrange
            var rand = new Random();
            var expectedSequence = new int[10];
            var queue = new MultiThreadedQueue<int>();
            //Act
            for (int i = 0; i < expectedSequence.Length; i++)
            {
                expectedSequence[i] = rand.Next();
                queue.Push(expectedSequence[i]);
            }
            var actualSequence = expectedSequence.Select(x => queue.Pop()).ToArray();
            //Assert
            Assert.That(actualSequence, Is.EqualTo(expectedSequence));
        }

        [Test]
        public async Task Queue_ShouldPushMultipleAsync_Success()
        {
            //Arrange
            var rand = new Random();
            var queue = new MultiThreadedQueue<int>();
            var writeTasks = new List<Task>();
            //Act
            for (int i = 0; i < 10; i++)
            {
                writeTasks.Add(Task.Run(async () =>
                {
                    await Task.Delay(500);
                    queue.Push(rand.Next(1, int.MaxValue));
                }));
            }
            await Task.WhenAll(writeTasks);

            var actualSequence = writeTasks.Select(x => queue.Pop()).ToArray();
            //Assert
            Assert.That(actualSequence.Length, Is.EqualTo(writeTasks.Count));
            Assert.That(actualSequence, Has.All.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task Queue_ShouldPopMultipleAsync_Success()
        {
            //Arrange
            var rand = new Random();
            var expectedSequence = new int[10];
            var queue = new MultiThreadedQueue<int>();
            for (int i = 0; i < expectedSequence.Length; i++)
            {
                expectedSequence[i] = rand.Next();
                queue.Push(expectedSequence[i]);
            }

            //Act
            var readTasks = expectedSequence.Select(x => Task.Run(async () =>
            {
                await Task.Delay(500);
                return queue.Pop();
            }));
            var actualSequence = await Task.WhenAll(readTasks);
            //Assert
            Assert.That(actualSequence, Is.EquivalentTo(expectedSequence));
        }

        [Test]
        public async Task Queue_ShouldPopMultipleAsyncInParallel_Success()
        {
            //Arrange
            var sw = new Stopwatch();
            var rand = new Random();
            var expectedSequence = new int[10];
            var queue = new MultiThreadedQueue<int>();
            for (int i = 0; i < expectedSequence.Length; i++)
            {
                expectedSequence[i] = rand.Next();
                queue.Push(expectedSequence[i]);
            }

            //Act
            var readTasks = expectedSequence.Select(x => Task.Run(async () =>
            {
                var res = queue.Pop();
                await Task.Delay(1000);
                return res;
            }));
            sw.Start();
            var actualSequence = await Task.WhenAll(readTasks);
            sw.Stop();
            //Assert
            Assert.That(actualSequence, Is.EquivalentTo(expectedSequence));
            Assert.That(sw.Elapsed, Is.LessThan(TimeSpan.FromMilliseconds(1200))); //Each thread delay + 200 ms huge reserved
        }

        [Test]
        public void Queue_CreateShouldThrow_IncorrectMaxLength()
        {
            //Arrange
            var maxLength = 0;
            //Act/assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new MultiThreadedQueue<int>(maxLength));
        }
    }
}
