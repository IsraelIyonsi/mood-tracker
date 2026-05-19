<?php

declare(strict_types=1);

namespace MoodTracker\Php;

use PDO;
use PDOException;
use RuntimeException;

final class SqliteConnection
{
    public static function open(string $databasePath): PDO
    {
        if (!is_readable($databasePath)) {
            throw new RuntimeException("Mood database not readable at {$databasePath}");
        }

        try {
            $pdo = new PDO("sqlite:{$databasePath}");
            $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
            $pdo->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_ASSOC);
            return $pdo;
        } catch (PDOException $exception) {
            throw new RuntimeException("Failed to open SQLite: {$exception->getMessage()}", 0, $exception);
        }
    }
}
