<?php

declare(strict_types=1);

namespace MoodTracker\Php;

use PDO;
use PDOException;

final class SqliteConnection
{
    public static function tryOpen(string $databasePath): ?PDO
    {
        if (!is_readable($databasePath)) {
            return null;
        }

        try {
            $pdo = new PDO("sqlite:{$databasePath}");
            $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
            $pdo->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_ASSOC);
            return $pdo;
        } catch (PDOException) {
            return null;
        }
    }
}
